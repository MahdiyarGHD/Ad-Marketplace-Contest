using System.Text.Json.Serialization;
using AdMarketplace.Bot;
using AdMarketplace.Domain.Options;
using AdMarketplace.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;

var bld = WebApplication.CreateBuilder();

bld.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
);

bld.Services.AddFastEndpoints(o =>
{
    o.Assemblies =
    [
        typeof(Program).Assembly,
        typeof(UpdateHandler).Assembly
    ];
});

bld.Services.SwaggerDocument(options =>
        options.AutoTagPathSegmentIndex = 2
);

bld.Services.ConfigureJsonSerializer();
bld.Services.ConfigureOptions(bld.Configuration);
bld.Services.ConfigureServices();
bld.Services.ConfigureHelpers();
bld.Services.ConfigureBotHandler();
bld.Services.ConfigureDbContexts(bld.Configuration);
bld.Services.ConfigureAuthentication(bld.Configuration);
bld.Services.ConfigureCors(bld.Configuration);
bld.Services.ConfigureTelegramBot(bld.Configuration);

var app = bld.Build();

await app.MigrateAndSeedAsync();

app.UseCustomExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(CorsOptions.PolicyName);

app.UseFastEndpoints();
app.UseSwaggerGen();
app.ConfigureAppStart();
app.Run();
