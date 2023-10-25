using API.Data;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration);
// in extension method: AddIdentityServices
builder.Services.AddIdentityServices(builder.Configuration);



var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Dotnet CLI can help us to create DB (can use in code at Program.cs)
using var scope = app.Services.CreateScope(); // this is going to give us access to all of the services that we have inside program class
var services = scope.ServiceProvider;
try
{
  var context = services.GetRequiredService<WebContext>();
  await context.Database.MigrateAsync();
  await Seed.UserSeedData(context);
}
catch (System.Exception ex)
{

  var logger = services.GetService<ILogger<Program>>();
  logger.LogError(ex, "An error occurred during migration");
}
app.Run();
