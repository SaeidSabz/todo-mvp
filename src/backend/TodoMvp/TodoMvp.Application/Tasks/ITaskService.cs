using TodoMvp.Application.Tasks.Models;

namespace TodoMvp.Application.Tasks
{
    public interface ITaskService
    {
        // Queries
        Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default);
        Task<TaskDto?> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default);

        // Commands
        Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
    }
}
