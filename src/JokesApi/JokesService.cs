using System.Text.Json;

namespace Company.Function.JokesApi;

/*
Api response:

{
    "categories":[],
    "created_at":"2020-01-05 13:42:30.177068",
    "icon_url":"https://api.chucknorris.io/img/avatar/chuck-norris.png",
    "id":"wqw0wSquQPqPwRuKQiAPbA",
    "updated_at":"2020-01-05 13:42:30.177068",
    "url":"https://api.chucknorris.io/jokes/wqw0wSquQPqPwRuKQiAPbA",
    "value":"Chuck Norris told the united states that russia was goin to nuc them during the cold war"
}
*/
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
        Console.WriteLine("VVV:" + joke.value);
        return joke?.value ?? throw new InvalidOperationException($"Could not deserialize: {body}");
    }
}