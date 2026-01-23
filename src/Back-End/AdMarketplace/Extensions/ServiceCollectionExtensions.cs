using System.Text.Json;
using AdMarketplace.Domain.Options;
using AdMarketplace.Infra.Helpers;

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

        public IServiceCollection ConfigureOptions(IConfiguration configuration)
        {
            services.Configure<TelegramBotOptions>(
                configuration.GetSection(TelegramBotOptions.KeyName));
        
            return services;
        }
    }
}