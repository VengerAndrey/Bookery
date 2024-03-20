using Bookery.Authentication.Repositories.User;
using Bookery.Authentication.Services.Common;
using Bookery.Authentication.Services.Hash;
using Bookery.Authentication.Services.JWT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IStsUserRepository, StsUserRepository>(_ =>
    new StsUserRepository(builder.Configuration.GetConnectionString("StorageAccount")));
builder.Services.AddSingleton<IHasher, Hasher>(_ => new Hasher(builder.Configuration["Salt"]));
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddHostedService<ExpiredTokenCleaner>();
builder.Services.AddSingleton<IHeaderService, HeaderService>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();