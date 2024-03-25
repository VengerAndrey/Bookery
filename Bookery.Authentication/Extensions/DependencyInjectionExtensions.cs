using System.Text;
using Bookery.Authentication.Data;
using Bookery.Authentication.Repositories.Implementations;
using Bookery.Authentication.Repositories.Interfaces;
using Bookery.Authentication.Services.Hosted;
using Bookery.Authentication.Services.Implementations;
using Bookery.Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bookery.Authentication.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddSingleton<IJwtService, JwtService>();
        serviceCollection.AddSingleton<IHasher, Hasher>(_ => new Hasher(configuration["Salt"]));
        serviceCollection.AddHostedService<ExpiredTokenCleaner>();

        serviceCollection.AddSingleton<AppConfiguration>();

        serviceCollection.AddDbContext(configuration);

        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Authentication:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Authentication:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.Unicode.GetBytes(configuration["Authentication:SigningKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
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
}