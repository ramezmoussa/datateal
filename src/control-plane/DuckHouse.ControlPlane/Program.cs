using DuckHouse.ControlPlane.Application;
using DuckHouse.ControlPlane.Application.InactivityEviction;
using DuckHouse.ControlPlane.Endpoints;
using DuckHouse.ControlPlane.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.Configure<InactivityEvictionOptions>(
    builder.Configuration.GetSection("InactivityEviction"));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();
app.MapNodeEndpoints();

app.Run();
