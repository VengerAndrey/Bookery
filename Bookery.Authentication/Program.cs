using Bookery.Authentication.Repositories.User;
using Bookery.Authentication.Services.Common;
using Bookery.Authentication.Services.Hash;
using Bookery.Authentication.Services.JWT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IStsUserRepository, StsUserRepository>(_ =>
    new StsUserRepository(builder.Configuration.GetConnectionString("StorageAccount")));
builder.Services.AddSingleton<IHasher, Hasher>(_ => new Hasher(builder.Configuration["Salt"]));
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddHostedService<ExpiredTokenCleaner>();
builder.Services.AddSingleton<IHeaderService, HeaderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();