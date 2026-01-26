using System.Text.Json;
using AdMarketplace.Database;
using AdMarketplace.Domain.Options;
using AdMarketplace.Infra.Helpers;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureServices()
        {
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