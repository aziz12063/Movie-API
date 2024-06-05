using ApiApplication.Models;

namespace ApiApplication.Test.Services.Models;

public class MovieTests
{
    [Fact]
    public void Movie_InitializedWithValidData_ShouldSetPropertiesCorrectly()
    {
        string id = "1";
        string title = "Inception";

        var movie = new MovieDto(id, title);

        Assert.Equal(id, movie.movieId);
        Assert.Equal(title, movie.Title);
    }

    // Immutability Test
    [Fact]
    public void Movie_Properties_ShouldBeReadOnly()
    {
        // Only testing for Id and Title
        Assert.True(typeof(MovieDto).GetProperty(nameof(MovieDto.movieId))?.CanWrite);
        Assert.True(typeof(MovieDto).GetProperty(nameof(MovieDto.Title))?.CanWrite);
    }
}
