using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoMvp.Application.Tasks;

namespace TodoMvp.Application.Configurations
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationDependencies(IConfiguration configuration)
            {
                services.AddScoped<ITaskService, TaskService>();
                return services;
            }
        }
    }
}
