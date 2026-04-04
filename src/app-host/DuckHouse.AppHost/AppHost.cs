var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DuckHouse_ControlPlane_Api>("api");

builder.Build().Run();
