using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TodoMvp.Application.Configurations
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationDependencies(IConfiguration configuration)
            {
                return services;
            }
        }
    }
}
