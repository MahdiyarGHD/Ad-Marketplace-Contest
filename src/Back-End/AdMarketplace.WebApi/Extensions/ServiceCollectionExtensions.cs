using System.Text.Json;
using System.Text.Json.Serialization;
using AdMarketplace.Bot;
using AdMarketplace.Database;
using AdMarketplace.Domain.Options;
using AdMarketplace.Infra.Helpers;
using AdMarketplace.Infra.Interfaces;
using AdMarketplace.Infra.Queues;
using AdMarketplace.Infra.Services.AgentServices;
using AdMarketplace.Infra.Services.CampaignServices;
using AdMarketplace.Infra.Services.CategoryServices;
using AdMarketplace.Infra.Services.ChannelServices;
using AdMarketplace.Infra.Services.TelegramServices;
using AdMarketplace.Infra.Services.UserServices;
using AdMarketplace.Workers.ChannelAnalytics;
using AdMarketplace.Workers.DealLifecycle;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Telegram.Bot;
using Telegram.Bot.AspNetCore;

namespace AdMarketplace.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureServices()
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserChannelConnectionService, UserChannelConnectionService>();
            services.AddScoped<IChannelPricingService, ChannelPricingService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IClientFactory, ClientFactory>();
            services.AddScoped<IAnalyticsUpdateService, AnalyticsUpdateService>();
            services.AddScoped<IPostingService, PostingService>();
            
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<ICampaignApplicationService, CampaignApplicationService>();
            services.AddScoped<ICampaignInvitationService, CampaignInvitationService>();
            services.AddScoped<IChannelApplicationService, ChannelApplicationService>();
            services.AddScoped<IDealService, DealService>();
            
            services.AddSingleton<IAnalyticsUpdateQueue, AnalyticsUpdateQueue>();
            services.AddHostedService<AnalyticsConsumerWorker>();
            services.AddHostedService<DealAutoCancelWorker>();
            services.AddHostedService<AutoPostingWorker>();
            services.AddHostedService<PostVerificationWorker>();

            return services;
        }

        public IServiceCollection ConfigureBotHandler()
        {
            services.Scan(scan => scan
                .FromAssemblyOf<UpdateHandler>()
                .AddClasses(classes => classes.AssignableToAny(typeof(IHandler), typeof(IHandlerWithResult<>))));

            services.AddScoped<UpdateHandler>();
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
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
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

        public IServiceCollection ConfigureTelegramBot(IConfiguration configuration)
        {
            var botConfigSection = configuration.GetSection(TelegramBotOptions.KeyName).Get<TelegramBotOptions>();

            if (botConfigSection is null)
                throw new InvalidConfigurationException();

            var telegramBotClientOptions = new TelegramBotClientOptions(token: botConfigSection.Token, baseUrl: botConfigSection.BotApiServer);

            services.AddHttpClient("TgWebhook")
                .RemoveAllLoggers()
                .ConfigureHttpClient(_ => { })
                .AddTypedClient<ITelegramBotClient>(
                    httpClient => new TelegramBotClient(telegramBotClientOptions, httpClient));

            services.ConfigureTelegramBotMvc();
            services.ConfigureTelegramBot<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt => opt.SerializerOptions);

            return services;
        }

        public IServiceCollection ConfigureOptions(IConfiguration configuration)
        {
            services.Configure<TelegramApiOptions>(
                configuration.GetSection(TelegramApiOptions.KeyName));

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
                        policy.WithOrigins([.. corsOptions.Origins]);

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
