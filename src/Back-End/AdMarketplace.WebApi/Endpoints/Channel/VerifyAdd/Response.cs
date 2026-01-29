namespace AdMarketplace.Endpoints.Channel.VerifyAdd;

public class Response
{
    public bool HasNewChannel { get; set; }
    public List<long> ChatIds { get; set; } = [];
}

