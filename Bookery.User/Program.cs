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
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb")));

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

app.Run();