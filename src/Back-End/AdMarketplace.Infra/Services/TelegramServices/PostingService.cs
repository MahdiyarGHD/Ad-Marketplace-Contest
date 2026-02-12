using System.Security.Cryptography;
using System.Text;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TL;
using Error = ErrorOr.Error;
using Message = TL.Message;

namespace AdMarketplace.Infra.Services.TelegramServices;

public class PostingService(
    ITelegramBotClient botClient,
    IClientFactory clientFactory,
    ILogger<PostingService> logger) : IPostingService
{
    public async Task<ErrorOr<long>> PostToChannelAsync(
        long channelChatId,
        long fromChatId,
        long messageId,
        CancellationToken ct = default)
    {
        try
        {
            var copied = await botClient.CopyMessage(
                new ChatId(channelChatId),
                new ChatId(fromChatId),
                (int)messageId,
                cancellationToken: ct);

            logger.LogInformation(
                "Posted message to channel {ChannelChatId}, new message ID: {NewMessageId}",
                channelChatId, copied.Id);

            return (long)copied.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to post to channel {ChannelChatId}", channelChatId);
            return Error.Failure("Posting.Failed", $"Failed to post to channel: {ex.Message}");
        }
    }

    public async Task<ErrorOr<PostVerificationResult>> VerifyPostAsync(
        Guid agentId,
        string channelUsername,
        long messageId,
        string? expectedTextHash,
        CancellationToken ct = default)
    {
        WTelegram.Client? client = null;

        try
        {
            var clientResult = await clientFactory.CreateClientAsync(agentId);
            if (clientResult.IsError)
                return Error.Failure("Posting.ClientFailed", "Failed to create MTProto client");

            client = clientResult.Value;
            await client.LoginUserIfNeeded();

            var resolved = await client.Contacts_ResolveUsername(channelUsername);
            var tlChannel = resolved?.chats?.Values.FirstOrDefault() as TL.Channel;
            if (tlChannel is null)
                return Error.Failure("Posting.ChannelNotFound", "Could not resolve channel");

            var inputChannel = new InputChannel(tlChannel.ID, tlChannel.access_hash);
            var result = await client.Channels_GetMessages(inputChannel, new InputMessageID { id = (int)messageId });

            if (result is not Messages_ChannelMessages channelMessages || channelMessages.Messages.Length == 0)
                return new PostVerificationResult(Exists: false, WasEdited: false, ViewCount: 0);

            var msg = channelMessages.Messages[0];

            if (msg is MessageEmpty)
                return new PostVerificationResult(Exists: false, WasEdited: false, ViewCount: 0);

            if (msg is not Message message)
                return new PostVerificationResult(Exists: false, WasEdited: false, ViewCount: 0);

            var wasEdited = false;
            if (expectedTextHash is not null)
            {
                var currentHash = ComputeHash(message.message ?? "");
                wasEdited = currentHash != expectedTextHash;
            }
            else if (message.edit_date != default)
            {
                wasEdited = true;
            }

            return new PostVerificationResult(Exists: true, WasEdited: wasEdited, ViewCount: message.views);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Post verification failed for channel {Channel}, message {MessageId}",
                channelUsername, messageId);
            return Error.Failure("Posting.VerificationFailed", $"Verification failed: {ex.Message}");
        }
        finally
        {
            client?.Dispose();
        }
    }

    public static string ComputeHash(string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexStringLower(bytes);
    }

    public async Task<ErrorOr<int>> GetPostViewCountAsync(
        Guid agentId,
        string channelUsername,
        long messageId,
        CancellationToken ct = default)
    {
        WTelegram.Client? client = null;

        try
        {
            var clientResult = await clientFactory.CreateClientAsync(agentId);
            if (clientResult.IsError)
                return Error.Failure("Posting.ClientFailed", "Failed to create MTProto client");

            client = clientResult.Value;
            await client.LoginUserIfNeeded();

            var resolved = await client.Contacts_ResolveUsername(channelUsername);
            var tlChannel = resolved?.chats?.Values.FirstOrDefault() as TL.Channel;
            if (tlChannel is null)
                return Error.Failure("Posting.ChannelNotFound", "Could not resolve channel");

            var inputChannel = new InputChannel(tlChannel.ID, tlChannel.access_hash);
            var result = await client.Channels_GetMessages(inputChannel, new InputMessageID { id = (int)messageId });

            if (result is not Messages_ChannelMessages channelMessages || channelMessages.Messages.Length == 0)
                return Error.NotFound("Posting.MessageNotFound", "Message not found");

            var msg = channelMessages.Messages[0];
            if (msg is not Message message)
                return Error.NotFound("Posting.MessageNotFound", "Message not found");

            return message.views;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get view count for channel {Channel}, message {MessageId}",
                channelUsername, messageId);
            return Error.Failure("Posting.ViewCountFailed", $"Failed to get view count: {ex.Message}");
        }
        finally
        {
            client?.Dispose();
        }
    }

    public async Task<ErrorOr<string>> GetMessageTextHashAsync(
        Guid agentId,
        string channelUsername,
        long messageId,
        CancellationToken ct = default)
    {
        WTelegram.Client? client = null;

        try
        {
            var clientResult = await clientFactory.CreateClientAsync(agentId);
            if (clientResult.IsError)
                return Error.Failure("Posting.ClientFailed", "Failed to create MTProto client");

            client = clientResult.Value;
            await client.LoginUserIfNeeded();

            var resolved = await client.Contacts_ResolveUsername(channelUsername);
            var tlChannel = resolved?.chats?.Values.FirstOrDefault() as TL.Channel;
            if (tlChannel is null)
                return Error.Failure("Posting.ChannelNotFound", "Could not resolve channel");

            var inputChannel = new InputChannel(tlChannel.ID, tlChannel.access_hash);
            var result = await client.Channels_GetMessages(inputChannel, new InputMessageID { id = (int)messageId });

            if (result is not Messages_ChannelMessages channelMessages || channelMessages.Messages.Length == 0)
                return Error.NotFound("Posting.MessageNotFound", "Message not found");

            var msg = channelMessages.Messages[0];
            if (msg is not Message message)
                return Error.NotFound("Posting.MessageNotFound", "Message not found");

            return ComputeHash(message.message ?? "");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get message hash for channel {Channel}, message {MessageId}",
                channelUsername, messageId);
            return Error.Failure("Posting.HashFailed", $"Failed to get message hash: {ex.Message}");
        }
        finally
        {
            client?.Dispose();
        }
    }

    public async Task<ErrorOr<bool>> DeletePostAsync(
        long channelChatId,
        long messageId,
        CancellationToken ct = default)
    {
        try
        {
            await botClient.DeleteMessage(
                new ChatId(channelChatId),
                (int)messageId,
                cancellationToken: ct);

            logger.LogInformation(
                "Deleted message {MessageId} from channel {ChannelChatId}",
                messageId, channelChatId);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete message {MessageId} from channel {ChannelChatId}",
                messageId, channelChatId);
            return Error.Failure("Posting.DeleteFailed", $"Failed to delete post: {ex.Message}");
        }
    }
}
