using Bookery.Storage.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IStorageService, BlobStorageService>(_ =>
    new BlobStorageService(
        builder.Configuration.GetConnectionString("StorageAccount"),
        builder.Configuration["BlobContainer"]));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();