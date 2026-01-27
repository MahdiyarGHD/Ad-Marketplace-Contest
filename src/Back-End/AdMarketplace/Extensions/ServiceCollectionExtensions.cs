using System.Text.Json;
using AdMarketplace.Database;
using AdMarketplace.Domain.Options;
using AdMarketplace.Infra.Helpers;
using AdMarketplace.Infra.Interfaces;
using AdMarketplace.Infra.Services;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureServices()
        {
            services.AddScoped<IUserService, UserService>();
            
            return services;
        }
        
        public IServiceCollection ConfigureHelpers()
        {
            services.AddSingleton<InitDataHelper>();
            return services;
        }

        public IServiceCollection ConfigureJsonSerializer()
        {
            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
        
            return services;
        }

        public IServiceCollection ConfigureDbContexts(IConfiguration configuration)
        {
            services.AddDbContext<AdMarketDbContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.UseNpgsql(configuration.GetConnectionString(AdMarketDbContextSchema.DefaultConnectionStringName));
            });

            return services;
        }
        
        public IServiceCollection ConfigureOptions(IConfiguration configuration)
        {
            services.Configure<TelegramBotOptions>(
                configuration.GetSection(TelegramBotOptions.KeyName));
        
            services.Configure<JwtOptions>(
                configuration.GetSection(JwtOptions.KeyName));

            services.Configure<CorsOptions>(
                configuration.GetSection(CorsOptions.KeyName));
        
            return services;
        }

        public IServiceCollection ConfigureCors(IConfiguration configuration)
        {
            var corsOptions = configuration.GetSection(CorsOptions.KeyName).Get<CorsOptions>() ?? new CorsOptions();
            var allowAnyOrigin = corsOptions.Origins.Count == 0 || corsOptions.Origins.Contains("*");

            services.AddCors(options =>
            {
                options.AddPolicy(CorsOptions.PolicyName, policy =>
                {
                    if (allowAnyOrigin)
                        policy.AllowAnyOrigin();
                    else
                        policy.WithOrigins(corsOptions.Origins.ToArray());

                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
            });

            return services;
        }
        
        public IServiceCollection ConfigureAuthentication(IConfiguration configuration)
        {
            services.AddAuthenticationJwtBearer(s => 
                    s.SigningKey = configuration.GetValue<string>($"{JwtOptions.KeyName}:SigningKey") ?? throw new ArgumentNullException()
                );
            services.AddAuthorization();
            return services;
        }
    }
}