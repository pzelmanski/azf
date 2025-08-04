
public interface IJokesApi
{
    public Task<string> GetRandomJokeAsync();
}

public class JokesApiMatchilling(HttpClient client) : IJokesApi
{
    public async Task<string> GetRandomJokeAsync()
    {
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

        return body;
    }
}
