using API.Extentions;
using API.Interfaces;
using API.Middleware;
using API.Services;

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

app.Run();
