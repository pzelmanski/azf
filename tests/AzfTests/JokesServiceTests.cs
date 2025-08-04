using Company.Function.JokesApi;
using FluentAssertions;
using LanguageExt;
using Moq;

namespace AzfTests;

public class JokesServiceTests
{
    [Fact]
    public async Task GivenJokeApiResponse_ItShouldDeserializeAndReturnJokeAsString()
    {
        var jokesApiMock = new Mock<IJokesApi>();
        var apiResponse = """
                          {
                          "categories":[],
                          "created_at":"2020-01-05 13:42:30.177068",
                          "icon_url":"https://api.chucknorris.io/img/avatar/chuck-norris.png",
                          "id":"wqw0wSquQPqPwRuKQiAPbA",
                          "updated_at":"2020-01-05 13:42:30.177068",
                          "url":"https://api.chucknorris.io/jokes/wqw0wSquQPqPwRuKQiAPbA",
                          "value":"Chuck Norris told the united states that russia was goin to nuc them during the cold war"
                          }
                          """;
        jokesApiMock.Setup(x => x.GetRandomJokeAsync()).ReturnsAsync(apiResponse);

        var sut = new JokesService(jokesApiMock.Object);
        
        var result = await sut.GetJokeAsync();
        result.Should().Be("Chuck Norris told the united states that russia was goin to nuc them during the cold war");

    }
}