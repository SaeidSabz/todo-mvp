using Moq;
using TodoMvp.Application.Tasks;
using TodoMvp.Application.Tasks.Models;
using TodoMvp.Domain.Entities;
using TodoMvp.Domain.Repositories;

namespace TodoMvp.Api.Tests.Application
{
    /// <summary>
    /// Unit tests for <see cref="TaskService"/> using a mocked <see cref="ITaskRepository"/> (Moq).
    /// </summary>
    [TestFixture]
    public sealed class TaskServiceTests
    {
        private Mock<ITaskRepository> _repoMock = default!;
        private ITaskService _service = default!;

        /// <summary>
        /// Initializes a fresh mock repository and service instance for each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<ITaskRepository>(MockBehavior.Strict);
            _service = new TaskService(_repoMock.Object);
        }

        /// <summary>
        /// Verifies that GetTasksAsync returns an empty list when no tasks exist.
        /// </summary>
        [Test]
        public async Task GetTasksAsync_ReturnsEmptyList_WhenNoTasksExist()
        {
            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<TaskItem>());

            var result = await _service.GetTasksAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that GetTasksAsync returns tasks when tasks exist.
        /// </summary>
        [Test]
        public async Task GetTasksAsync_ReturnsTasks_WhenTasksExist()
        {
            var tasks = new[]
            {
            new TaskItem { Id = 1, Title = "Task A", Description = "Desc A", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TaskItem { Id = 2, Title = "Task B", Description = "Desc B", IsCompleted = true,  CreatedAt = DateTime.UtcNow }
        };

            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tasks);

            var result = await _service.GetTasksAsync();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(x => x.Title), Does.Contain("Task A"));
            Assert.That(result.Select(x => x.Title), Does.Contain("Task B"));

            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that GetTaskByIdAsync returns the task when it exists.
        /// </summary>
        [Test]
        public async Task GetTaskByIdAsync_ReturnsTask_WhenExists()
        {
            var task = new TaskItem
            {
                Id = 10,
                Title = "Task X",
                Description = "Desc",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            var result = await _service.GetTaskByIdAsync(10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(10));
            Assert.That(result.Title, Is.EqualTo("Task X"));

            _repoMock.Verify(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that GetTaskByIdAsync returns null when the task does not exist.
        /// </summary>
        [Test]
        public async Task GetTaskByIdAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(999999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var result = await _service.GetTaskByIdAsync(999999);

            Assert.That(result, Is.Null);

            _repoMock.Verify(r => r.GetByIdAsync(999999, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that CreateTaskAsync creates a task with the expected fields.
        /// </summary>
        [Test]
        public async Task CreateTaskAsync_CreatesTask_WithExpectedFields()
        {
            var due = DateTime.UtcNow.AddDays(3);
            TaskItem? captured = null;

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Callback<TaskItem, CancellationToken>((t, _) =>
                {
                    captured = t;
                    // Simulate DB-generated Id
                    t.Id = 1;
                })
                .ReturnsAsync(() => captured!);

            var result = await _service.CreateTaskAsync(new CreateTaskRequest("New Task", "Desc", due));

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("New Task"));
            Assert.That(result.Description, Is.EqualTo("Desc"));
            Assert.That(result.DueDate, Is.EqualTo(due));
            Assert.That(result.IsCompleted, Is.False);

            _repoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that UpdateTaskAsync returns true and updates fields when the task exists.
        /// </summary>
        [Test]
        public async Task UpdateTaskAsync_ReturnsTrue_AndUpdatesFields_WhenExists()
        {
            var existing = new TaskItem
            {
                Id = 5,
                Title = "Original",
                Description = "Old",
                IsCompleted = false,
                DueDate = null,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = null
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _repoMock
                .Setup(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var update = new UpdateTaskRequest(
                Title: "Updated",
                Description: "New",
                IsCompleted: true,
                DueDate: DateTime.UtcNow.AddDays(2));

            var result = await _service.UpdateTaskAsync(5, update);

            Assert.That(result, Is.True);

            // Verify the entity was mutated before repository update
            Assert.That(existing.Title, Is.EqualTo("Updated"));
            Assert.That(existing.Description, Is.EqualTo("New"));
            Assert.That(existing.IsCompleted, Is.True);
            Assert.That(existing.DueDate, Is.EqualTo(update.DueDate));

            _repoMock.Verify(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that UpdateTaskAsync returns false when the task does not exist.
        /// </summary>
        [Test]
        public async Task UpdateTaskAsync_ReturnsFalse_WhenNotFound()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(999999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            var update = new UpdateTaskRequest("Updated", null, false, null);

            var result = await _service.UpdateTaskAsync(999999, update);

            Assert.That(result, Is.False);

            _repoMock.Verify(r => r.GetByIdAsync(999999, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that DeleteTaskAsync returns true and deletes the task when it exists.
        /// </summary>
        [Test]
        public async Task DeleteTaskAsync_ReturnsTrue_WhenExists()
        {
            _repoMock
                .Setup(r => r.DeleteByIdAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.DeleteTaskAsync(3);

            Assert.That(result, Is.True);

            _repoMock.Verify(r => r.DeleteByIdAsync(3, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }


        /// <summary>
        /// Verifies that DeleteTaskAsync returns false when the task does not exist.
        /// </summary>
        [Test]
        public async Task DeleteTaskAsync_ReturnsFalse_WhenNotFound()
        {
            _repoMock
                .Setup(r => r.DeleteByIdAsync(999999, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _service.DeleteTaskAsync(999999);

            Assert.That(result, Is.False);

            _repoMock.Verify(r => r.DeleteByIdAsync(999999, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.VerifyNoOtherCalls();
        }


    }
}