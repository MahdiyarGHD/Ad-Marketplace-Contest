using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Contracts.Responses;

namespace AdMarketplace.Endpoints.Channel.GetByCategory;

public class Response
{
    public required List<ChannelResponseContract> Channels { get; set; }
}
