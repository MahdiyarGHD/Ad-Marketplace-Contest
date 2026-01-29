using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Category.GetById;

public class Endpoint(ICategoryService categoryService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/categories/{Id}");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await categoryService.GetByIdAsync(req.Id);

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Id = result.Value.Id,
            Name = result.Value.Name,
            Description = result.Value.Description,
            Icon = result.Value.Icon,
            DisplayOrder = result.Value.DisplayOrder
        };
    }
}

