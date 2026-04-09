var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
var controlPlaneDb = postgres.AddDatabase("duckhouse-control-plane");
var uiDb = postgres.AddDatabase("duckhouse-ui");

var controlPlane = builder.AddProject<Projects.DuckHouse_ControlPlane>("control-plane")
    .WithReference(controlPlaneDb)
    .WaitFor(controlPlaneDb);

builder.AddProject<Projects.DuckHouse_Ui_Server>("ui")
    .WithReference(controlPlane)
    .WithReference(uiDb)
    .WaitFor(uiDb);

builder.Build().Run();
