using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// When ConnectionStrings:duckhouse-ui is present in AppHost configuration (e.g. appsettings.Development.json),
// Aspire forwards those connection strings to the services instead of starting a local Postgres container.
// This allows running all services through Aspire while targeting external Azure resources.
IResourceBuilder<IResourceWithConnectionString> controlPlaneDb;
IResourceBuilder<IResourceWithConnectionString> uiDb;

if (builder.Configuration.GetConnectionString("duckhouse-ui") is not null)
{
    controlPlaneDb = builder.AddConnectionString("duckhouse-control-plane");
    uiDb = builder.AddConnectionString("duckhouse-ui");
}
else
{
    var postgres = builder.AddPostgres("postgres")
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);
    controlPlaneDb = postgres.AddDatabase("duckhouse-control-plane");
    uiDb = postgres.AddDatabase("duckhouse-ui");
}

var controlPlane = builder.AddProject<Projects.DuckHouse_ControlPlane>("control-plane")
    .WithReference(controlPlaneDb)
    .WaitFor(controlPlaneDb);

var orchestrator = builder.AddProject<Projects.DuckHouse_Orchestrator>("orchestrator")
    .WithReference(uiDb)
    .WithReference(controlPlane)
    .WaitFor(uiDb)
    .WaitFor(controlPlane);

builder.AddProject<Projects.DuckHouse_Ui_Server>("ui")
    .WithReference(controlPlane)
    .WithReference(orchestrator)
    .WithReference(uiDb)
    .WaitFor(uiDb);

builder.Build().Run();
