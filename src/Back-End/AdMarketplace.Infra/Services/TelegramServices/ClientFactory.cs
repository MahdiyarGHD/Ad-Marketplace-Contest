using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Options;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WTelegram;

namespace AdMarketplace.Infra.Services.TelegramServices;

public class ClientFactory(
    AdMarketDbContext dbContext,
    IOptions<TelegramApiOptions> telegramApiOptions) : IClientFactory
{
    public async Task<ErrorOr<Client>> CreateClientAsync(Guid agentId)
    {
        var agent = await dbContext.Agents.FirstOrDefaultAsync(a => a.Id == agentId);
        if (agent is null)
            return Error.NotFound("Agent.NotFound", "Agent not found");

        var credentials = telegramApiOptions.Value.Credentials;
        if (credentials.Count == 0)
            return Error.Failure("TelegramApi.NoCredentials", "No Telegram API credentials configured");

        var random = new Random();
        var selectedCredential = credentials[random.Next(credentials.Count)];

        var client = new Client(what => ClientConfig(what, selectedCredential, agent));
        return client;
    }
    
    private static string? ClientConfig(string what, TelegramApiCredential credential, Agent agent) =>  
    what switch
        {
            "session_pathname" => agent.SessionPath,
            "api_id" => credential.ApiId,
            "api_hash" => credential.ApiHash,
            "phone_number" => agent.PhoneNumber,
            _ => null
        };
}
