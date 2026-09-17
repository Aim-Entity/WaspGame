using Application.Abstracts.Repositories;
using Infrastructure.DatabaseContext;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ConfigureServices
    {
        public const string DefaultDatabaseName = "WaspGame";

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            string? databaseName = null)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName ?? DefaultDatabaseName);
            });

            services.AddScoped<IGameRepository, GameRepository>();

            return services;
        }
    }
}
