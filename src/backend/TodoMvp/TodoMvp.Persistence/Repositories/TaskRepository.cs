using Microsoft.EntityFrameworkCore;
using TodoMvp.Domain.Entities;
using TodoMvp.Domain.Repositories;
using TodoMvp.Persistence.Data;

namespace TodoMvp.Persistence.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="ITaskRepository"/>.
    /// Responsible for persisting and retrieving <see cref="TaskItem"/> entities using <see cref="TodoMvpDbContext"/>.
    /// </summary>
    public sealed class TaskRepository : ITaskRepository
    {
        private readonly TodoMvpDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The EF Core database context.</param>
        public TaskRepository(TodoMvpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc cref="ITaskRepository.GetAllAsync(CancellationToken)"/>
        public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc cref="ITaskRepository.GetByIdAsync(int, CancellationToken)"/>
        public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        /// <inheritdoc cref="ITaskRepository.AddAsync(TaskItem, CancellationToken)"/>
        public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(task);

            await _dbContext.Tasks.AddAsync(task, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return task;
        }

        /// <inheritdoc cref="ITaskRepository.UpdateAsync(TaskItem, CancellationToken)"/>
        public async Task<bool> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(task);

            var existing = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);
            if (existing is null)
            {
                return false;
            }

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.IsCompleted = task.IsCompleted;
            existing.DueDate = task.DueDate;
            existing.UpdatedAt = task.UpdatedAt;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <inheritdoc cref="ITaskRepository.DeleteByIdAsync(TaskItem, CancellationToken)"/>
        public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            if (existing is null)
            {
                return false;
            }

            _dbContext.Tasks.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;

        }
    }
}
