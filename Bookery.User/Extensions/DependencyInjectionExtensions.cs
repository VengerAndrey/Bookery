using Bookery.Authentication.Common.Client;
using Bookery.Common.DomainEvents;
using Bookery.Node.Common.Client;
using Bookery.Storage.Common.Client;
using Bookery.User.Data;
using Bookery.User.Data.DomainEvents;
using Bookery.User.Repositories.Implementations;
using Bookery.User.Repositories.Interfaces;
using Bookery.User.Services.Handlers;
using Bookery.User.Services.Implementations;
using Bookery.User.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookery.User.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IPhotoService, PhotoService>();

        serviceCollection.AddDbContext(configuration);
        serviceCollection.AddClients(configuration);
        serviceCollection.AddDomainEventServices();
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
        serviceCollection.AddScoped<IAuthenticationApiClient, AuthenticationApiClient>(x =>
            new(configuration["AuthenticationApi:BaseUrl"]));
        serviceCollection.AddScoped<INodeApiClient, NodeApiClient>(x =>
            new(configuration["NodeApi:BaseUrl"]));
        serviceCollection.AddScoped<IStorageApiClient, StorageApiClient>(x =>
            new(configuration["StorageApi:BaseUrl"]));
    }

    private static void AddDomainEventServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        serviceCollection.AddScoped<IDomainEventHandler<AuthenticationUserCreated>, AuthenticationUserCreatedHandler>();
        serviceCollection.AddScoped<IDomainEventHandler<NodeUserCreated>, NodeUserCreatedHandler>();
    }
}