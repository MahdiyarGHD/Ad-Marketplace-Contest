using AdMarketplace.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument(options => options.AutoTagPathSegmentIndex = 2);

var app = bld.Build();
app.UseCustomExceptionHandler();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();