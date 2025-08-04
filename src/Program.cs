using Company.Function.JokesApi;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddHttpClient()
    .AddSingleton<IDbRepository, DbRepository>()
    .AddSingleton<IDbService, DbService>()
    .AddSingleton<IJokesApi, JokesApiMatchilling>()
    .AddSingleton<IJokesService, JokesService>();

builder.Build().Run();
