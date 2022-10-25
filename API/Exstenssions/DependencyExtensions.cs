using Core.Interfaces;
using Infrastructure.Repository;

namespace API.Exstenssions
{
    public static class DependencyExtensions
    {
        public static IServiceCollection DependencyExtend(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IFileStorageRepository, AppStorageRepository>();

            return services;
        }
    }
}
