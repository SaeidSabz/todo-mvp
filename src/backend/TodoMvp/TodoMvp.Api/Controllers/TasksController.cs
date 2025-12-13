using Microsoft.AspNetCore.Mvc;
using TodoMvp.Application.Tasks;
using TodoMvp.Application.Tasks.Models;

namespace TodoMvp.Api.Controllers
{

    /// <summary>
    /// Provides CRUD endpoints for managing tasks.
    /// </summary>
    [ApiController]
    [Route("api/tasks")]
    [Produces("application/json")]
    public sealed class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TasksController"/> class.
        /// </summary>
        /// <param name="taskService">The application service responsible for task use-cases.</param>
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Retrieves all tasks.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of tasks.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll(CancellationToken cancellationToken)
        {
            var tasks = await _taskService.GetTasksAsync(cancellationToken);
            return Ok(tasks);
        }

        /// <summary>
        /// Retrieves a single task by its identifier.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The matching task if found; otherwise 404.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);

            if (task is null)
            {
                return NotFound(new { error = "NotFound", message = $"Task with id '{id}' was not found." });
            }

            return Ok(task);
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="request">The task creation request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created task.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { error = "ValidationFailed", details = new[] { "Title is required." } });
            }

            var created = await _taskService.CreateTaskAsync(request, cancellationToken);

            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = created.Id },
                value: created);
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <param name="request">The task update request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>No content if updated; otherwise 404 or 400.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { error = "ValidationFailed", details = new[] { "Title is required." } });
            }

            var updated = await _taskService.UpdateTaskAsync(id, request, cancellationToken);

            if (!updated)
            {
                return NotFound(new { error = "NotFound", message = $"Task with id '{id}' was not found." });
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes an existing task.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>No content if deleted; otherwise 404.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _taskService.DeleteTaskAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound(new { error = "NotFound", message = $"Task with id '{id}' was not found." });
            }

            return NoContent();
        }
    }

}
