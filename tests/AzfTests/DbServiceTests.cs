using LanguageExt;
using LanguageExt.Common;
using Moq;
using FluentAssertions;

namespace AzfTests;

public class DbServiceTests
{
    // What I would also do in prod application is to 
    // write "unit-integration" tests with docker DB and
    // TestContainers, but they are too much effort to implement
    // them in this exercise. I would possibly replace all this tests below
    // with the ones using docker db (and aim to create a nice API to be easily
    // able to implement more tests with db)
    
    [Fact]
    public async Task GivenJokeOver200chars_ItShouldReturnError()
    {
        var repo = new Mock<IDbRepository>().Object;
        var sut = new DbService(repo);

        var over200CharsString =
            "over 200 chars string: aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        var result = await sut.TryInsertJokeAsync(over200CharsString);

        result.Match(
            r => Unit.Default,
            l => throw new Exception("Expected error, got OK"));
    }

    [Fact]
    public async Task GivenDuplicateJoke_ItShouldReturnError()
    {
        var repoMock = new Mock<IDbRepository>();
        repoMock.Setup(x => x.AlreadyExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        var sut = new DbService(repoMock.Object);
        
        var result = await sut.TryInsertJokeAsync("already exists");
        result.IsRight.Should().BeTrue("Expected error, got OK");

    }
    
    [Fact]
    public async Task GivenNonDuplicateJokeUnder200Chars_ItShouldInsert()
    {
        var repoMock = new Mock<IDbRepository>();
        repoMock.Setup(x => x.AlreadyExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var sut = new DbService(repoMock.Object);

        var joke = "joke to be inserted";
        var result = await sut.TryInsertJokeAsync(joke);
        result.Match(
            r => throw new Exception($"Expected OK, got Error: {r}"),
            l => repoMock.Verify(x => x.InsertJokeAsync(joke)));
    } 
}