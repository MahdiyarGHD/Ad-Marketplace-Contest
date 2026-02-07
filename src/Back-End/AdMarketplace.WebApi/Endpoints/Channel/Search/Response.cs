using AdMarketplace.Domain.Contracts.Responses;

namespace AdMarketplace.Endpoints.Channel.Search;

public class Response
{
    public required List<ChannelResponseContract> Channels { get; set; }
}
