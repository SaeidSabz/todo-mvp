using Microsoft.EntityFrameworkCore;
using Entities = TodoMvp.Domain.Entities;

namespace TodoMvp.Persistence.Data
{
    public class TodoMvpDbContext : DbContext
    {
        public TodoMvpDbContext(DbContextOptions<TodoMvpDbContext> options)
            : base(options)
        {
        }

        public DbSet<Entities.Task> Tasks => Set<Entities.Task>();

        public override int SaveChanges()
        {
            ApplyTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTimestamps()
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<Entities.Task>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = utcNow;
                }
            }
        }
    }
}
