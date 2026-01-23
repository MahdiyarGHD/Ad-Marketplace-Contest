using FastEndpoints;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddFastEndpoints()
    .SwaggerDocument(options => options.AutoTagPathSegmentIndex = 2);

var app = bld.Build();
app.UseFastEndpoints()
    .UseSwaggerGen();
app.Run();