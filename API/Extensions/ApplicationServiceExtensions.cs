using System;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options => options.UseMySql(connectionString: config.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 21))));


            return services;
        }
    }
}