using System.Text.Json;

namespace Company.Function.JokesApi;

public class JokeResponse
{
    public string value { get; init; }
}

public interface IJokesService
{
    public Task<string> GetJokeAsync();
}

public class JokesService(IJokesApi api) : IJokesService
{
    public async Task<string> GetJokeAsync()
    {
        var body = await api.GetRandomJokeAsync();
        var joke = JsonSerializer.Deserialize<JokeResponse>(body);
        return joke?.value ?? throw new InvalidOperationException($"Could not deserialize: {body}");
    }
}