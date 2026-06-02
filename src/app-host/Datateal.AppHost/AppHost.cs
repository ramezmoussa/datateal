using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// When ConnectionStrings:datateal-ui is present in AppHost configuration (e.g. appsettings.Development.json),
// Aspire forwards those connection strings to the services instead of starting a local Postgres container.
// This allows running all services through Aspire while targeting external Azure resources.
IResourceBuilder<IResourceWithConnectionString> controlPlaneDb;
IResourceBuilder<IResourceWithConnectionString> uiDb;

if (builder.Configuration.GetConnectionString("datateal-ui") is not null)
{
    controlPlaneDb = builder.AddConnectionString("datateal-control-plane");
    uiDb = builder.AddConnectionString("datateal-ui");
}
else
{
    var postgres = builder.AddPostgres("postgres")
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);
    controlPlaneDb = postgres.AddDatabase("datateal-control-plane");
    uiDb = postgres.AddDatabase("datateal-ui");
}

var controlPlane = builder.AddProject<Projects.Datateal_ControlPlane>("control-plane")
    .WithReference(controlPlaneDb)
    .WaitFor(controlPlaneDb);

var orchestrator = builder.AddProject<Projects.Datateal_Orchestrator>("orchestrator")
    .WithReference(uiDb)
    .WithReference(controlPlane)
    .WaitFor(uiDb)
    .WaitFor(controlPlane);

builder.AddProject<Projects.Datateal_Ui_Server>("ui")
    .WithReference(controlPlane)
    .WithReference(orchestrator)
    .WithReference(uiDb)
    .WaitFor(uiDb);

builder.Build().Run();
