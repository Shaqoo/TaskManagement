using Application.Abstractions;
using Application.Extensions;
using Application.Features.Tasks.CreateTask;
using Application.Features.Tasks.GetMyTasks;
using Domain.Entities;
using Host.Extensions;
using Host.Hubs;
using Host.Service;
using Infrastructures.Extensions;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Setup
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetMyTasksQuery).Assembly);

});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddScoped<INotifier, Notifier>();

var app = builder.Build();

app.UseCustomMiddlewares();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});


app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHub<NotificationHub>("/NotificationHub");

app.Run();
