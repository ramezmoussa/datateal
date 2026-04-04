var builder = DistributedApplication.CreateBuilder(args);

var controlPlane = builder.AddProject<Projects.DuckHouse_ControlPlane>("control-plane");

builder.AddProject<Projects.DuckHouse_Ui>("ui")
    .WithReference(controlPlane);

builder.Build().Run();
