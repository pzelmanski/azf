
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
        var isDuplicate = await repo.IsDuplicate(joke);
        if (isDuplicate)
            return Either<Unit, string>.Right("Duplicate joke");
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