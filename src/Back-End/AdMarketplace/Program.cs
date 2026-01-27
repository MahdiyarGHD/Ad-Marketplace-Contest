using AdMarketplace;
using AdMarketplace.Database;
using AdMarketplace.Domain.Options;
using AdMarketplace.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();

bld.Services.SwaggerDocument(options =>
    options.AutoTagPathSegmentIndex = 2);

bld.Services.ConfigureJsonSerializer();
bld.Services.ConfigureServices();
bld.Services.ConfigureHelpers();
bld.Services.ConfigureDbContexts(bld.Configuration);
bld.Services.ConfigureOptions(bld.Configuration);
bld.Services.ConfigureAuthentication(bld.Configuration);
bld.Services.ConfigureCors(bld.Configuration);

var app = bld.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseCustomExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(CorsOptions.PolicyName);

app.UseFastEndpoints();

app.UseSwaggerGen();

app.Run();
