using API.Data;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extentions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration config)
    {
        // Add the database for application
        services.AddDbContext<WebContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        // adding CORS support in the API, because angular can access the server.(policy builder)
        services.AddCors();
        // Add Service to ServiceCollection
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
        // add Mail Settings
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        services.AddTransient<ISendMailService, SendMailService>();

        return services;
    }
}