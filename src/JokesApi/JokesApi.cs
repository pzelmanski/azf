
public interface IJokesApi
{
    public Task<string> GetRandomJoke();
}

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

public class JokesApiMatchilling : IJokesApi
{
    public async Task<string> GetRandomJoke()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random"),
            Headers =
            {
                { "x-rapidapi-key", "45d96b2378mshbe1b6c7ccca44efp1c6487jsne3b81d5a951e" },
                { "x-rapidapi-host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com" },
                { "accept", "application/json" },
            },
        };
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine(body);

        return body;
    }
}


