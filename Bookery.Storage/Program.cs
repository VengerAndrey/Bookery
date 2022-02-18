using Bookery.Storage.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IStorageService, BlobStorageService>(_ =>
    new BlobStorageService(
        builder.Configuration.GetConnectionString("StorageAccount"),
        builder.Configuration["BlobContainer"]));

var rabbitMq = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddHostedService<StorageConsumer>(provider => new StorageConsumer(rabbitMq["Host"],
    Convert.ToInt32(rabbitMq["Port"]), rabbitMq["Username"],
    rabbitMq["Password"], provider.GetRequiredService<IStorageService>()));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();