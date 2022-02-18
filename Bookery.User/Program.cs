using Bookery.User.Data;
using Bookery.User.Repositories.Main;
using Bookery.User.Repositories.Node;
using Bookery.User.Repositories.STS;
using Bookery.User.Services.Common;
using Bookery.User.Services.Hash;
using Bookery.User.Services.Photo;
using Bookery.User.Services.User;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    var server = builder.Configuration["ServerName"];
    var database = builder.Configuration["Database"];
    var password = builder.Configuration["Password"];

    options.UseSqlServer($"Server={server};Database={database};User=sa;Password={password};");
});

builder.Services.AddHttpClient("NodeClient",
    config => { config.BaseAddress = new Uri(builder.Configuration["NodeUrl"].TrimEnd('/') + "/"); });
builder.Services.AddHttpClient("StorageClient",
    config => { config.BaseAddress = new Uri(builder.Configuration["StorageUrl"].TrimEnd('/') + "/"); });

builder.Services.AddSingleton<IHasher, Hasher>(_ => new Hasher(builder.Configuration["Salt"]));
builder.Services.AddSingleton<IStsUserRepository, StsUserRepository>(_ =>
    new StsUserRepository(builder.Configuration.GetConnectionString("StorageAccount")));
builder.Services.AddSingleton<IMainUserRepository, MainUserRepository>();
builder.Services.AddSingleton<INodeUserRepository, NodeUserRepository>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IHeaderService, HeaderService>();
builder.Services.AddSingleton<IPhotoService, PhotoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();