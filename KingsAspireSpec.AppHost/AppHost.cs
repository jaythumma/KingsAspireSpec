var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var customerService = builder.AddProject<Projects.KingsAspireSpec_CustomerService>("customerservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.KingsAspireSpec_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(customerService)
    .WaitFor(customerService);
builder.Build().Run();
