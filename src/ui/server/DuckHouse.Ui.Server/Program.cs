using System.Text.Json.Serialization;
using DuckHouse.Ui.Server;
using DuckHouse.Ui.Server.Application;
using DuckHouse.Ui.Server.Components;
using DuckHouse.Ui.Server.Infrastructure;
using DuckHouse.Ui.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers()
	.AddJsonOptions(options =>
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddExceptionHandler<UpstreamExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.AddNpgsqlDbContext<UiDbContext>("duckhouse-ui");

builder.Services.AddOrchestratorProxy();

builder.Services.AddAntDesign();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UiDbContext>();
    await db.Database.CreateExecutionStrategy().ExecuteAsync(() => db.Database.MigrateAsync());
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	app.UseExceptionHandler(); // picks up UpstreamExceptionHandler
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true); // picks up UpstreamExceptionHandler, then falls back to /Error
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapOrchestratorProxy();
app.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(DuckHouse.Ui.Client._Imports).Assembly);

app.MapDefaultEndpoints();

app.Run();