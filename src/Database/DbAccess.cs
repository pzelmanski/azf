
using LanguageExt;


public interface IDbAccess
{
    public Task<Either<Unit, string>> TryInsertJoke(string joke);
}

public class DbAccess(IDbRepository repo) : IDbAccess
{
    private readonly IDbRepository repo = repo;

    public async Task<Either<Unit, string>> TryInsertJoke(string joke)
    {
        if (joke.Length > CONSTS.MaxJokeLength)
            return Either<Unit, string>.Right($"Joke exceeded limit of {CONSTS.MaxJokeLength} characters.");

        // Assumption: there's only one thread & one instance running at the time and there's no race condition possible.
        // In case its not true, I would join exist check & insert into single command / db transaction
        var isDuplicate = await repo.AlreadyExists(joke);
        if (isDuplicate)
            return Either<Unit, string>.Right("Joke already exists.");
        try
        {
            await repo.InsertJoke(joke);

            return Either<Unit, string>.Left(Unit.Default);
        }
        catch (System.Exception e)
        {
            return Either<Unit, string>.Right(e.ToString());
            throw;
        }
    }
}