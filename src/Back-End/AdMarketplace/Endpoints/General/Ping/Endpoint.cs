using EasyMicroservices.ServiceContracts;
using FastEndpoints;

namespace AdMarketplace.Endpoints.General.Ping;

public class MyEndpoint : EndpointWithoutRequest<MessageContract>
{
    public override void Configure()
    {
        Get("/api/general/ping");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(true, ct);
    }
}