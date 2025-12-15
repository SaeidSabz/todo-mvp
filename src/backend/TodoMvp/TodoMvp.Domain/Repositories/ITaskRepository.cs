using TodoMvp.Domain.Entities;

namespace TodoMvp.Domain.Repositories
{
    public interface ITaskRepository
    {
        /// <summary>
        /// Retrieves all tasks ordered by Id ascending.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A read-only list of tasks.</returns>
        Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a single task by its unique identifier.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The matching task, or <c>null</c> if not found.</returns>
        Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken);


        /// <summary>
        /// Adds a new task and persists changes.
        /// </summary>
        /// <param name="task">The task to add.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created task (including generated Id).</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="task"/> is null.</exception>
        Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing task and persists changes.
        /// </summary>
        /// <param name="task">The task to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="task"/> is null.</exception>
        Task<bool> UpdateAsync(TaskItem existing, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a task by its identifier.
        /// </summary>
        /// <param name="id">The task identifier.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>True if the task existed and was deleted; otherwise false.</returns>
        Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}