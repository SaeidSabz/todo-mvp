using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoMvp.Domain.Repositories;
using TodoMvp.Persistence.Data;
using TodoMvp.Persistence.Repositories;

namespace TodoMvp.Persistence.Configurations
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddPersistenceDependencies(IConfiguration configuration)
            {
                services.AddDbContext<TodoMvpDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TodoMvpDb");
                });
                services.AddScoped<ITaskRepository, TaskRepository>();

                return services;
            }
        }
    }
}
