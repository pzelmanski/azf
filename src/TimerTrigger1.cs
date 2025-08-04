using Company.Function.JokesApi;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;

namespace Company.Function;

public class TimerTrigger1(
    IDbService dbService,
    IDbRepository dbRepository,
    IJokesService jokesService,
    ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<TimerTrigger1>();

    public async Task<int> RunJokesInsertAsync()
    {
        var insertedJokesCount = 0;

        // circuit breaker implemented just in case there are no jokes under 200 chars - to avoid infinite loop
        // and skyrocketing costs

        // TODO: Add test for jokes count && circuit breaker - possibly move this function into separate class
        for (var circutBreaker = 0; insertedJokesCount < CONSTS.JokesToInsertCount && circutBreaker < CONSTS.JokesToInsertCount * 2; circutBreaker++)
        {
            var joke = await jokesService.GetJokeAsync();
            var result = await dbService.TryInsertJokeAsync(joke);
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
    public async Task Run([TimerTrigger($"0 */1 * * * *")] TimerInfo myTimer)
    {
        try
        {
            _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

            // TODO: It would probably be better to hide DB init inside DbService in order not to leak DbRepository into top level function
            await dbRepository.InitializeDbAsync();

            var insertedJokesCount = await RunJokesInsertAsync();

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