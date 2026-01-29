using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Category.List;

public class Endpoint(ICategoryService categoryService)
    : EndpointWithoutRequest<ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/categories");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(CancellationToken ct)
    {
        var result = await categoryService.GetAllAsync();

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Categories = result.Value.Select(c => new CategoryItem
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Icon = c.Icon,
                DisplayOrder = c.DisplayOrder
            }).ToList()
        };
    }
}

