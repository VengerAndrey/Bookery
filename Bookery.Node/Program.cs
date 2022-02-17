using Bookery.Node.Data;
using Bookery.Node.Services.Common;
using Bookery.Node.Services.Node;
using Bookery.Node.Services.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = false);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb")));

builder.Services.AddHttpClient("StorageClient",
    config => { config.BaseAddress = new Uri(builder.Configuration["StorageUrl"].TrimEnd('/') + "/"); });

builder.Services.AddSingleton<IHeaderService, HeaderService>();
builder.Services.AddSingleton<INodeService, NodeService>();
builder.Services.AddSingleton<IUserNodeService, UserNodeService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IStorageService, StorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();