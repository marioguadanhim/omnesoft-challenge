var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var database = postgres.AddDatabase("OmnesoftChallengeDB");

var api = builder.AddProject<Projects.OmnesoftChallenge_Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithHttpHealthCheck("/health");

builder.AddViteApp("frontend", "../../../frontend")
    .WithEndpoint("http", endpoint =>
    {
        endpoint.Port = 5173;
        endpoint.IsProxied = false;
    })
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
