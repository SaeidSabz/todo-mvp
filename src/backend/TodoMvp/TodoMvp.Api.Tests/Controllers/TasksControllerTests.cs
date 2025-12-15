using System.Net;
using System.Net.Http.Json;
using TodoMvp.Api.Contracts.Errors;
using TodoMvp.Api.Tests.TestInfrastructure;
using TodoMvp.Application.Tasks.Models;

namespace TodoMvp.Api.Tests.Controllers
{
    /// <summary>
    /// Integration tests for TasksController CRUD endpoints.
    /// </summary>
    [TestFixture]
    public sealed class TasksControllerTests
    {
        private TodoMvpWebApplicationFactory _factory = default!;
        private HttpClient _client = default!;

        /// <summary>
        /// Sets up a fresh test server and HttpClient before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _factory = new TodoMvpWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        /// <summary>
        /// Disposes the test server and HttpClient after each test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        /// <summary>
        /// GET /api/tasks returns 200 OK and an empty list when no tasks exist.
        /// </summary>
        [Test]
        public async Task GetAll_ReturnsOkAndEmptyList_WhenNoTasksExist()
        {
            var response = await _client.GetAsync("/api/tasks");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks, Has.Count.EqualTo(0));
        }

        /// <summary>
        /// GET /api/tasks returns 200 OK and a list when tasks exist.
        /// </summary>
        [Test]
        public async Task GetAll_ReturnsOkAndTasks_WhenTasksExist()
        {
            await CreateTaskAsync("Task A");
            await CreateTaskAsync("Task B");

            var response = await _client.GetAsync("/api/tasks");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks, Has.Count.EqualTo(2));
            Assert.That(tasks!.Select(t => t.Title), Does.Contain("Task A"));
            Assert.That(tasks.Select(t => t.Title), Does.Contain("Task B"));
        }

        /// <summary>
        /// GET /api/tasks/{id} returns 200 OK when the task exists.
        /// </summary>
        [Test]
        public async Task GetById_ReturnsOk_WhenTaskExists()
        {
            var created = await CreateTaskAsync("Task X");

            var response = await _client.GetAsync($"/api/tasks/{created.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var task = await response.Content.ReadFromJsonAsync<TaskDto>();
            Assert.That(task, Is.Not.Null);
            Assert.That(task!.Id, Is.EqualTo(created.Id));
            Assert.That(task.Title, Is.EqualTo("Task X"));
        }

        /// <summary>
        /// GET /api/tasks/{id} returns 404 NotFound when the task does not exist.
        /// </summary>
        [Test]
        public async Task GetById_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            var response = await _client.GetAsync("/api/tasks/999999");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error, Is.EqualTo("NotFound"));
        }

        /// <summary>
        /// POST /api/tasks returns 201 Created when request is valid.
        /// </summary>
        [Test]
        public async Task Create_ReturnsCreated_WhenRequestIsValid()
        {
            var request = new CreateTaskRequest("New Task", "Desc", DateTime.UtcNow.AddDays(1));

            var response = await _client.PostAsJsonAsync("/api/tasks", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var created = await response.Content.ReadFromJsonAsync<TaskDto>();
            Assert.That(created, Is.Not.Null);
            Assert.That(created!.Id, Is.GreaterThan(0));
            Assert.That(created.Title, Is.EqualTo("New Task"));

            // Best-practice check: Location header should be present for 201 Created
            Assert.That(response.Headers.Location, Is.Not.Null);
        }

        /// <summary>
        /// POST /api/tasks returns 400 BadRequest when title is missing.
        /// </summary>
        [Test]
        public async Task Create_ReturnsBadRequest_WhenTitleIsMissing()
        {
            // Title is required; send an invalid payload
            var request = new { description = "Desc", dueDate = DateTime.UtcNow.AddDays(1) };

            var response = await _client.PostAsJsonAsync("/api/tasks", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error, Is.EqualTo("ValidationFailed"));
            Assert.That(error.Details, Is.Not.Null);
            Assert.That(error.Details!, Has.Count.GreaterThan(0));
        }

        /// <summary>
        /// POST /api/tasks returns 400 BadRequest when title exceeds max length.
        /// </summary>
        [Test]
        public async Task Create_ReturnsBadRequest_WhenTitleExceedsMaxLength()
        {
            var longTitle = new string('a', 201);
            var request = new CreateTaskRequest(longTitle, "Desc", null);

            var response = await _client.PostAsJsonAsync("/api/tasks", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error, Is.EqualTo("ValidationFailed"));
        }

        /// <summary>
        /// PUT /api/tasks/{id} returns 204 NoContent when update succeeds.
        /// </summary>
        [Test]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var created = await CreateTaskAsync("Task To Update");

            var update = new UpdateTaskRequest("Updated Title", "Updated Desc", true, DateTime.UtcNow.AddDays(2));

            var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", update);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var getResponse = await _client.GetAsync($"/api/tasks/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<TaskDto>();

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.Title, Is.EqualTo("Updated Title"));
            Assert.That(updated.IsCompleted, Is.True);
        }

        /// <summary>
        /// PUT /api/tasks/{id} returns 404 NotFound when task does not exist.
        /// </summary>
        [Test]
        public async Task Update_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            var update = new UpdateTaskRequest("Updated", null, false, null);

            var response = await _client.PutAsJsonAsync("/api/tasks/999999", update);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error, Is.EqualTo("NotFound"));
        }

        /// <summary>
        /// PUT /api/tasks/{id} returns 400 BadRequest when request is invalid.
        /// </summary>
        [Test]
        public async Task Update_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            var created = await CreateTaskAsync("Valid Title");

            // Title required: empty title should fail validation
            var update = new UpdateTaskRequest(string.Empty, null, false, null);

            var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", update);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error, Is.EqualTo("ValidationFailed"));
        }

        /// <summary>
        /// DELETE /api/tasks/{id} returns 204 NoContent when delete succeeds.
        /// </summary>
        [Test]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            var created = await CreateTaskAsync("Task To Delete");

            var deleteResponse = await _client.DeleteAsync($"/api/tasks/{created.Id}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var getResponse = await _client.GetAsync($"/api/tasks/{created.Id}");
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        /// <summary>
        /// DELETE /api/tasks/{id} returns 404 NotFound when task does not exist.
        /// </summary>
        [Test]
        public async Task Delete_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            var response = await _client.DeleteAsync("/api/tasks/999999");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Error, Is.EqualTo("NotFound"));
        }

        /// <summary>
        /// Creates a task via the public API.
        /// </summary>
        /// <param name="title">The task title.</param>
        /// <returns>The created task DTO.</returns>
        private async Task<TaskDto> CreateTaskAsync(string title)
        {
            var request = new CreateTaskRequest(title, "Desc", DateTime.UtcNow.AddDays(1));
            var response = await _client.PostAsJsonAsync("/api/tasks", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var created = await response.Content.ReadFromJsonAsync<TaskDto>();
            Assert.That(created, Is.Not.Null);

            return created!;
        }
    }

}
