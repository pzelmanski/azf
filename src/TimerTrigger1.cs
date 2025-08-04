using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;

namespace Company.Function;

public class TimerTrigger1(
    IDbAccess dbAccess,
    IDbRepository dbRepository,
    IJokesApi jokesApi,
    ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<TimerTrigger1>();
    private readonly IDbAccess dbAccess = dbAccess;
    private readonly IJokesApi jokesApi = jokesApi;
    private readonly IDbRepository dbRepository = dbRepository;

    public async Task<int> RunJokesInsertAsync(IJokesApi api, IDbAccess db)
    {
        var insertedJokesCount = 0;

        // circuit breaker implemented just in case there are no jokes under 200 chars - to avoid infinite loop
        // and skyrocketing costs
        for (var circutBreaker = 0; insertedJokesCount < CONSTS.JokesToInsertCount && circutBreaker < CONSTS.JokesToInsertCount * 2; circutBreaker++)
        {
            var joke = await jokesApi.GetRandomJoke();
            var result = await dbAccess.TryInsertJokeAsync(joke);
            result.Match(
                e => _logger.LogInformation("could not insert joke because: {Error}", e),
                ok =>
                {
                    _logger.LogInformation("Inserted joke: {Joke}", joke);
                    insertedJokesCount++;
                });
        }
        return insertedJokesCount;
    }

    // Assumption: in case of failure, we just wait X minutes and skip the separate flow for the failed case.
    // if it would be an issue, we could store timestamp of last successful run in db and fetch more jokes
    // in case job failed on previos run
    [Function("TimerTrigger1")]
    public async Task Run([TimerTrigger($"0 */5 * * * *")] TimerInfo myTimer)
    {
        try
        {
            _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

            await dbRepository.InitializeDbAsync();

            // this could (possibly should?) be IoC via default .net services, but I dont feel like learning
            // how to implment it in azure functions - sorry guys its 11pm and I have to wake up in the morning
            var insertedJokesCount = await RunJokesInsertAsync(jokesApi, dbAccess);

            _logger.LogInformation("Inserted {InsertedCount} jokes", insertedJokesCount);

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
            }

            // For monitoring purpose I'd add a count metric in a form of:
            // _metrics.SendSuccess()
            // which would send a metric of JokesFunction.Success = 1, so that we
            // can see easily if it succeeded
        }
        catch (System.Exception e)
        {
            _logger.LogCritical(e, "Application failed");

            // here I'd also send job failed metric
            // or have alert on critical errors - on failure we want to know
            // that it failed right away
            throw;
        }
    }
}