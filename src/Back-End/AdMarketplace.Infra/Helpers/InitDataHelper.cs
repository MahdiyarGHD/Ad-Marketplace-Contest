using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Options;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace AdMarketplace.Infra.Helpers;

public class InitDataHelper(IOptions<TelegramBotOptions> botOptions, JsonSerializerOptions serializerOptions)
{
    public async Task<ErrorOr<InitDataUserContract>> ValidateInitDataAsync(string initData)
    {
        var botToken = botOptions.Value.Token;
        var data = HttpUtility.ParseQueryString(initData);
        
        if(IsDataExpired(data["auth_date"]))
            return Error.Forbidden("The init data is expired.");
        
        var hash = data["hash"];

        data.Remove("hash");

        var checkString = string.Join("\n", data.AllKeys.OrderBy(key => key)
            .Select(key => $"{key}={data[key]}")
        );
        var hmacKey = new HMACSHA256("WebAppData"u8.ToArray());
        var secretKey = hmacKey.ComputeHash(Encoding.UTF8.GetBytes(botToken));
        var hashKey = new HMACSHA256(secretKey);
        var hashBytes = hashKey.ComputeHash(Encoding.UTF8.GetBytes(checkString));
        var computedHash = Convert.ToHexStringLower(hashBytes);

        if (!computedHash.Equals(hash))
            return Error.Forbidden("The init data is invalid.");

        using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(data["user"] ?? string.Empty));
        var initDataContract = await JsonSerializer.DeserializeAsync<InitDataUserContract>(inputStream, serializerOptions);

        if(initDataContract is null)
            return Error.Forbidden();
        
        return initDataContract;
    }
    
    private static bool IsDataExpired(string? authTimestamp)
    {
        if (string.IsNullOrWhiteSpace(authTimestamp) || !long.TryParse(authTimestamp, out var dateUnix))
            return true;

        var dateTime = DateTimeOffset.FromUnixTimeSeconds(dateUnix).UtcDateTime;
        return (DateTimeOffset.UtcNow - dateTime).TotalSeconds > 14 * 24 * 3600; // ToDo: after implementing mini app, reduce this to something like 60s
    } 
}