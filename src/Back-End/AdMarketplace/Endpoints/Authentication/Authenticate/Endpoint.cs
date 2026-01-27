using System.Security.Claims;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Options;
using AdMarketplace.Infra.Helpers;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.Extensions.Options;

namespace AdMarketplace.Endpoints.Authentication.Authenticate;

public class Endpoint(
    InitDataHelper initDataHelper,
    IOptions<JwtOptions> jwtOptions, 
    IUserService userService,
    ILogger<Endpoint> logger)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/authentication/authenticate");
        AllowAnonymous();
    }
    
    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var validationResult = await initDataHelper.ValidateInitDataAsync(req.InitData);
        if (validationResult.IsError)
            return validationResult.Errors;

        await userService.EnsureExistsAsync(validationResult.Value);
        
        var jwtSetting = jwtOptions.Value;
        var jwtToken = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtSetting.SigningKey;
            options.Audience = jwtSetting.Audience;
            options.Issuer = jwtSetting.Issuer;
            options.ExpireAt = DateTime.UtcNow.Add(jwtSetting.ExpireAt);
            
            options.User.Claims.Add(
                new Claim("UserId", validationResult.Value.Id.ToString())
            );
        });

        var result = new Response
        {
            AccessToken = jwtToken,
        };
        
        return result;
    }
}