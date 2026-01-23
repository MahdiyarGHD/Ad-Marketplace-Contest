using AdMarketplace.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument(options => options.AutoTagPathSegmentIndex = 2);
bld.Services.ConfigureJsonSerializer();
bld.Services.ConfigureServices();
bld.Services.ConfigureHelpers();
bld.Services.ConfigureOptions(bld.Configuration);

var app = bld.Build();
app.UseCustomExceptionHandler();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();