using Bookery.Node.Data;
using Bookery.Node.Services.Common;
using Bookery.Node.Services.Node;
using Bookery.Node.Services.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = false);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    var server = builder.Configuration["Postgresql:Server"];
    var database = builder.Configuration["Postgresql:Database"];
    var username = builder.Configuration["Postgresql:Username"];
    var password = builder.Configuration["Postgresql:Password"];

    options.UseNpgsql($"Host={server}; Database={database}; Username={username}; Password={password};");
});

builder.Services.AddHttpClient("StorageClient",
    config => { config.BaseAddress = new Uri(builder.Configuration["StorageUrl"].TrimEnd('/') + "/"); });

builder.Services.AddSingleton<IHeaderService, HeaderService>();
builder.Services.AddSingleton<INodeService, NodeService>();
builder.Services.AddSingleton<IUserNodeService, UserNodeService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IStorageService, StorageService>();

var rabbitMq = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddSingleton<IStorageProducer, StorageProducer>(_ =>
    new StorageProducer(rabbitMq["Host"], Convert.ToInt32(rabbitMq["Port"]), rabbitMq["Username"],
        rabbitMq["Password"]));

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

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