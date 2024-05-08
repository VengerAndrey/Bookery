using Bookery.Storage.Services.Hosted;
using Bookery.Storage.Services.Implementations;
using Bookery.Storage.Services.Interfaces;

namespace Bookery.Storage.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        if (bool.TryParse(configuration["UseLocalStorageService"], out var useLocalStorageService) &&
            useLocalStorageService)
        {
            serviceCollection.AddScoped<IStorageService, LocalStorageService>();
        }
        else
        {
            serviceCollection.AddScoped<IStorageService, BlobStorageService>(_ =>
                new BlobStorageService(
                    configuration["ConnectionStrings:StorageAccount"],
                    configuration["BlobContainer"]));
        }

        serviceCollection.AddRabbitMq(configuration);
    }

    private static void AddRabbitMq(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var hostname = configuration["RabbitMq:Host"];
        var port = int.TryParse(configuration["RabbitMq:Port"], out var parsedPort) ? parsedPort : 5672;
        var username = configuration["RabbitMq:Username"];
        var password = configuration["RabbitMq:Password"];
        var queue = configuration["RabbitMq:Queue"];
        
        serviceCollection.AddHostedService<StorageConsumer>(provider =>
            new StorageConsumer(provider.GetRequiredService<IServiceScopeFactory>(), hostname, port, username, password, queue));
    }
}