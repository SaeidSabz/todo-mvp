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
        public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(task);

            // Attach + mark modified to support scenarios where the entity came from a different context
            // or was loaded AsNoTracking (which we do in GetByIdAsync).
            _dbContext.Tasks.Attach(task);
            _dbContext.Entry(task).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc cref="ITaskRepository.DeleteAsync(TaskItem, CancellationToken)"/>
        public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(task);

            _dbContext.Tasks.Attach(task);
            _dbContext.Tasks.Remove(task);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
