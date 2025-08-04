
using Microsoft.Data.Sqlite;

public interface IDbRepository
{
    public Task<int> InitializeDbAsync();
    public Task<bool> AlreadyExistsAsync(string joke);
    public Task InsertJokeAsync(string joke);
}


public class DbRepository: IDbRepository
{
    public async Task<int> InitializeDbAsync()
    {
        using var connection = new SqliteConnection(CONSTS._connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        // Assumption: I dont need to store any IDs, I'm only interested in storing jokes
        // Assumption: Performance is not an issue, I'm adding unique constraint onto text field in db for additional safety

        // In case performance would be an issue, I could add hash of joke - in case of hash already existing I could compare if
        // its the same joke or just accidental hash collission
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS jokes (
                joke text unique not null
            );";

        var result = await command.ExecuteNonQueryAsync();
        return result;
    }

    public async Task<bool> AlreadyExistsAsync(string joke)
    {
        using var connection = new SqliteConnection(CONSTS._connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "select joke from jokes j where j.joke = @joke";
        command.Parameters.Add(new SqliteParameter("@joke", joke));

        var result = await command.ExecuteReaderAsync();
        return result.HasRows;
    }

    public async Task InsertJokeAsync(string joke)
    {
        using var connection = new SqliteConnection(CONSTS._connectionString);
        await connection.OpenAsync();
        var c2 = connection.CreateCommand();
        c2.CommandText = @$" insert into jokes (joke) values ('{joke}');";
        await c2.ExecuteNonQueryAsync();
    }
}