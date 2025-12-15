using TodoMvp.Application.Tasks.Models;
using TodoMvp.Domain.Entities;
using TodoMvp.Domain.Repositories;

namespace TodoMvp.Application.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default)
        {
            var tasks = await _taskRepository.GetAllAsync(cancellationToken);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
            return task is null ? null : MapToDto(task);
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                IsCompleted = false
            };

            var created = await _taskRepository.AddAsync(task, cancellationToken);
            return MapToDto(created);
        }

        public async Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var existing = await _taskRepository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                return false;
            }

            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.IsCompleted = request.IsCompleted;
            existing.DueDate = request.DueDate;

            await _taskRepository.UpdateAsync(existing, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _taskRepository.DeleteByIdAsync(id, cancellationToken);
        }

        private static TaskDto MapToDto(TaskItem task) =>
            new(
                task.Id,
                task.Title,
                task.Description,
                task.IsCompleted,
                task.DueDate,
                task.CreatedAt,
                task.UpdatedAt);
    }
}
