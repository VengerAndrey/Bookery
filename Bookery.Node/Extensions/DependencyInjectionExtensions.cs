using Bookery.Node.Data;
using Bookery.Node.Services.Implementations;
using Bookery.Node.Services.Interfaces;
using Bookery.Storage.Common.Client;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<INodeService, NodeService>();
        serviceCollection.AddScoped<IUserNodeService, UserNodeService>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IStorageService, StorageService>();
        
        serviceCollection.AddDbContext(configuration);
        serviceCollection.AddClients(configuration);
        serviceCollection.AddRabbitMq(configuration);
    }

    private static void AddDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContextFactory<AppDbContext>(options =>
        {
            var server = configuration["Postgresql:Server"];
            var database = configuration["Postgresql:Database"];
            var username = configuration["Postgresql:Username"];
            var password = configuration["Postgresql:Password"];

            options.UseNpgsql($"Host={server}; Database={database}; Username={username}; Password={password};");
        });
    }

    private static void AddClients(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<IStorageApiClient, StorageApiClient>(x =>
            new(configuration["StorageApi:BaseUrl"]));
    }

    private static void AddRabbitMq(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var hostname = configuration["RabbitMq:Host"];
        var port = int.TryParse(configuration["RabbitMq:Port"], out var parsedPort) ? parsedPort : 5672;
        var username = configuration["RabbitMq:Username"];
        var password = configuration["RabbitMq:Password"];
        var queue = configuration["RabbitMq:Queue"];

        serviceCollection.AddSingleton<IStorageProducer, StorageProducer>(_ =>
            new StorageProducer(hostname, port, username, password, queue));
    }
}