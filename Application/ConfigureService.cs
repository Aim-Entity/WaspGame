using Application.Abstracts;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Application
{
    public static class ConfigureService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.TryAddSingleton(TimeProvider.System);
            services.TryAddSingleton(Random.Shared);

            services.AddScoped<IGameService, GameService>();

            return services;
        }
    }
}
