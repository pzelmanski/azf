
using LanguageExt;


public interface IDbService
{
    public Task<Either<Unit, string>> TryInsertJokeAsync(string joke);
}

public class DbService(IDbRepository repo) : IDbService
{
    private readonly IDbRepository repo = repo;

    public async Task<Either<Unit, string>> TryInsertJokeAsync(string joke)
    {
        if (joke.Length > CONSTS.MaxJokeLength)
            return Either<Unit, string>.Right($"Joke exceeded limit of {CONSTS.MaxJokeLength} characters.");

        // Assumption: there's only one thread & one instance running at the time and there's no race condition possible.
        // In case its not true, I would join exist check & insert into single command / db transaction
        var isDuplicate = await repo.AlreadyExistsAsync(joke);
        if (isDuplicate)
            return Either<Unit, string>.Right("Joke already exists.");
        try
        {
            await repo.InsertJokeAsync(joke);

            return Either<Unit, string>.Left(Unit.Default);
        }
        catch (System.Exception e)
        {
            return Either<Unit, string>.Right(e.ToString());
            throw;
        }
    }
}