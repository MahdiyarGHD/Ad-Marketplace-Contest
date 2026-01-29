using AdMarketplace.Domain.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace AdMarketplace.Extensions;

public static class LifeTimeExtensions
{
    public static WebApplication ConfigureAppStart(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(async void () =>
        {
            using var scope = app.Services.CreateScope();
            var bot = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var botOptions = scope.ServiceProvider.GetRequiredService<IOptions<TelegramBotOptions>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                await bot.DeleteWebhook();

                await bot.SetWebhook(
                    botOptions.Value.WebhookUrl,
                    allowedUpdates: [UpdateType.Message, UpdateType.MyChatMember, UpdateType.ChatMember],
                    secretToken: botOptions.Value.SecretToken
                );

                logger.LogInformation("Webhook Url: {WebhookUrl}", botOptions.Value.WebhookUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error has occurred during setting webhook, make sure to set the appsettings correct.");
            }
        });

        return app;
    }
}