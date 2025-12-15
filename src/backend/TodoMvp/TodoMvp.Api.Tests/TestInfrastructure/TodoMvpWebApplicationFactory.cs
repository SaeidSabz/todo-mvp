using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoMvp.Persistence.Data;

namespace TodoMvp.Api.Tests.TestInfrastructure
{
    /// <summary>
    /// WebApplicationFactory that configures the API host for tests, including an isolated InMemory database.
    /// </summary>
    public sealed class TodoMvpWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"TodoMvpDb_Test_{Guid.NewGuid()}";

        /// <summary>
        /// Configures the web host used for tests.
        /// </summary>
        /// <param name="builder">The web host builder.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration (if any).
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TodoMvpDbContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                // Use a unique DB name per test factory instance to avoid cross-test leakage.
                services.AddDbContext<TodoMvpDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);

                });
            });
        }
    }

}
