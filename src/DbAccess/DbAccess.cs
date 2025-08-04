
using LanguageExt;
using Microsoft.Data.Sqlite;

public interface IDbAccess
{
    public Task<bool> IsDuplicate(string joke);
    public Task<Either<Unit, string>> TryInsertJoke(string joke);
    public Task<int> InitializeDbAsync();
}

public class DbAccess : IDbAccess
{

    public async Task<int> InitializeDbAsync()
    {
        using var connection = new SqliteConnection(CONSTS._connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        // Assumption: I dont need to store any IDs, I'm only interested in storing jokes
        // Assumption: Performance is not an issue, I'm adding unique constraint onto text field in db for additional safety
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS jokes (
                joke text unique not null
            );";

        var result = await command.ExecuteNonQueryAsync();
        return result;
    }

    public async Task<bool> IsDuplicate(string joke)
    {
        using var connection = new SqliteConnection(CONSTS._connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "select joke from jokes j where j.joke = @joke";
        command.Parameters.Add(joke);

        var result = await command.ExecuteReaderAsync();
        return result.HasRows;
    }

    public async Task<Either<Unit, string>> TryInsertJoke(string joke)
    {
        var isDuplicate = await IsDuplicate(joke);
        if (isDuplicate)
            return Either<Unit, string>.Right("Duplicate joke");
        try
        {
            using var connection = new SqliteConnection(CONSTS._connectionString);
            await connection.OpenAsync();
            var c2 = connection.CreateCommand();
            c2.CommandText = @$" insert into jokes (joke) values ('{joke}');";
            var insertedCount = await c2.ExecuteNonQueryAsync();

            return Either<Unit, string>.Left(Unit.Default);
        }
        catch (System.Exception e)
        {
            return Either<Unit, string>.Right(e.ToString());
            throw;
        }
    }
}