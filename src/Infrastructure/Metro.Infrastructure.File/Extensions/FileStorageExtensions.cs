using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Metro.Domain.Services;
using Metro.Infrastructure.File.Services;

namespace Metro.Infrastructure.File.Extensions
{
    public static class FileStorageExtensions
    {
        public static IServiceCollection AddFileStorage(this IServiceCollection services)
        {
            services.AddScoped<IFileStorageService, FileStorageService>();
            return services;
        }
    }
}
