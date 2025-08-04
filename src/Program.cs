using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// TODO: Implement ioc
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddHttpClient()
    .AddSingleton<IDbRepository, DbRepository>()
    .AddSingleton<IDbAccess, DbAccess>()
    .AddSingleton<IJokesApi, JokesApiMatchilling>();

builder.Build().Run();
