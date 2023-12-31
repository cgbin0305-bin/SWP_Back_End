using API.Data;
using API.Entities;
using API.Helper;
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
        // add the IOption CloudinarySettings base on which being setup in appsettings.json
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IChoresRepository,ChoresRepository>();
        return services;
    }
}