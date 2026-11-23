using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NewsService.WebHost.Helpers;
using NewsService.Infrastructure.EntityFramework;
using NewsService.Infrastructure.EntityFramework.RepositoriesEF;
using NewsService.Domain.Repositories.Abstractions;
using NewsService.Application.Services;
using NewsService.Application.Services.Abstractions;
using NewsService.Application.Services.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(nameof(ApplicationDbContext));

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string for NewsServiceDbContext is not configured.");
}

builder.Services.AddNpgsql<ApplicationDbContext>(connectionString, options =>
{
    options.MigrationsAssembly("NewsService.Infrastructure.EntityFramework");

});

builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "News service API",
            Description = "API for creating, viewing, storing, modifying, and deleting notes."
        });
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExceptionHandler<InvalidNewsStatusExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IAuthorRepository, EfAuthorRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<INewsRepository, EfNewsRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IReactionRepository, EfReactionRepository>();

builder.Services.AddScoped<IAuthorApplicationService, AuthorApplicationService>();
builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();
builder.Services.AddScoped<INewsApplicationService, NewsApplicationService>();
builder.Services.AddScoped<ICommentApplicationService, CommentApplicationService>();
builder.Services.AddScoped<IReactionApplicationService, ReactionApplicationService>();
builder.Services.AddAutoMapper(configuration =>
{
    configuration.AddProfile<ApplicationProfile>();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.MigrateDatabase<ApplicationDbContext>();

app.Run();