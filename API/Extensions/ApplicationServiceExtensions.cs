using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<PresenceTracker>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("JAWSDB_OLIVE_URL");

                    // Parse connection URL to connection string for mySql
                    connUrl = connUrl.Replace("mysql://", string.Empty);
                    var mySqlUserPass = connUrl.Split("@")[0];
                    var mySqlHostPortDb = connUrl.Split("@")[1];
                    var mySqlHostPort = mySqlHostPortDb.Split("/")[0];
                    var mySqlDb = mySqlHostPortDb.Split("/")[1];
                    var mySqlUser = mySqlUserPass.Split(":")[0];
                    var mySqlPass = mySqlUserPass.Split(":")[1];
                    var mySqlHost = mySqlHostPort.Split(":")[0];
                    var mySqlPort = mySqlHostPort.Split(":")[1];

                    connStr = $"Server={mySqlHost};Port={mySqlPort};User Id={mySqlUser};Password={mySqlPass};Database={mySqlDb}";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseMySql(connStr, new MySqlServerVersion(new Version(8, 0, 21)));
            });


            return services;
        }
    }
}