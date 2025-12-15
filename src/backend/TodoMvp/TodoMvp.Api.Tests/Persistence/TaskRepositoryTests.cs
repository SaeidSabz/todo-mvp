using Microsoft.EntityFrameworkCore;
using TodoMvp.Domain.Entities;
using TodoMvp.Persistence.Data;
using TodoMvp.Persistence.Repositories;

namespace TodoMvp.Api.Tests.Persistence
{
    /// <summary>
    /// Unit tests for <see cref="TaskRepository"/> using EF Core InMemory database.
    /// </summary>
    [TestFixture]
    public sealed class TaskRepositoryTests
    {
        private TodoMvpDbContext _dbContext = default!;
        private TaskRepository _repository = default!;

        /// <summary>
        /// Creates a fresh repository and InMemory database per test to ensure isolation.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _dbContext = CreateInMemoryDbContext();
            _repository = new TaskRepository(_dbContext);
        }

        /// <summary>
        /// Disposes the DbContext after each test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        /// <summary>
        /// Verifies that GetAllAsync returns an empty list when no tasks exist.
        /// </summary>
        [Test]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoTasksExist()
        {
            var tasks = await _repository.GetAllAsync();

            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks, Is.Empty);
        }

        /// <summary>
        /// Verifies that GetAllAsync returns all tasks when tasks exist.
        /// </summary>
        [Test]
        public async Task GetAllAsync_ReturnsAllTasks_WhenTasksExist()
        {
            await _repository.AddAsync(new TaskItem { Title = "Task A", CreatedAt = DateTime.UtcNow });
            await _repository.AddAsync(new TaskItem { Title = "Task B", CreatedAt = DateTime.UtcNow });

            var tasks = await _repository.GetAllAsync();

            Assert.That(tasks.Count, Is.EqualTo(2));
            Assert.That(tasks.Select(t => t.Title), Does.Contain("Task A"));
            Assert.That(tasks.Select(t => t.Title), Does.Contain("Task B"));
        }

        /// <summary>
        /// Verifies that GetByIdAsync returns the task when it exists.
        /// </summary>
        [Test]
        public async Task GetByIdAsync_ReturnsTask_WhenExists()
        {
            var created = await _repository.AddAsync(new TaskItem
            {
                Title = "Task X",
                Description = "Desc",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });

            var result = await _repository.GetByIdAsync(created.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(created.Id));
            Assert.That(result.Title, Is.EqualTo("Task X"));
            Assert.That(result.Description, Is.EqualTo("Desc"));
        }

        /// <summary>
        /// Verifies that GetByIdAsync returns null when the task does not exist.
        /// </summary>
        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var result = await _repository.GetByIdAsync(999999);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Verifies that AddAsync adds a task and assigns an identifier.
        /// </summary>
        [Test]
        public async Task AddAsync_AddsTask_AndAssignsId()
        {
            var task = new TaskItem
            {
                Title = "New Task",
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(task);

            Assert.That(created.Id, Is.GreaterThan(0));

            var all = await _repository.GetAllAsync();
            Assert.That(all.Count, Is.EqualTo(1));
            Assert.That(all[0].Title, Is.EqualTo("New Task"));
        }

        /// <summary>
        /// Verifies that UpdateAsync persists changes to an existing task.
        /// </summary>
        [Test]
        public async Task UpdateAsync_PersistsUpdatedFields()
        {
            var created = await _repository.AddAsync(new TaskItem
            {
                Title = "Original",
                Description = "Old",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });

            // Mimic typical flow: retrieve, mutate, update
            var entity = await _repository.GetByIdAsync(created.Id);
            Assert.That(entity, Is.Not.Null);

            entity!.Title = "Updated";
            entity.Description = "New";
            entity.IsCompleted = true;

            await _repository.UpdateAsync(entity);

            // Reload and verify persistence
            var updated = await _repository.GetByIdAsync(created.Id);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.Title, Is.EqualTo("Updated"));
            Assert.That(updated.Description, Is.EqualTo("New"));
            Assert.That(updated.IsCompleted, Is.True);
        }

        /// <summary>
        /// Verifies that DeleteAsync removes a task from the database.
        /// </summary>
        [Test]
        public async Task DeleteAsync_RemovesTask()
        {
            var created = await _repository.AddAsync(new TaskItem
            {
                Title = "To Delete",
                CreatedAt = DateTime.UtcNow
            });

            var entity = await _repository.GetByIdAsync(created.Id);
            Assert.That(entity, Is.Not.Null);

            await _repository.DeleteByIdAsync(entity.Id);

            var result = await _repository.GetByIdAsync(created.Id);
            Assert.That(result, Is.Null);

            var all = await _repository.GetAllAsync();
            Assert.That(all, Is.Empty);
        }

        /// <summary>
        /// Creates a new EF Core InMemory <see cref="TodoMvpDbContext"/> with a unique database name.
        /// </summary>
        /// <returns>A new <see cref="TodoMvpDbContext"/> instance.</returns>
        private static TodoMvpDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TodoMvpDbContext>()
                .UseInMemoryDatabase($"TodoMvpDb_TaskRepositoryTests_{Guid.NewGuid()}")
                .Options;

            return new TodoMvpDbContext(options);
        }
    }

}
