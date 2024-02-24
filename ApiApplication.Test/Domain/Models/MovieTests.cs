using ApiApplication.Domain.Models;

namespace ApiApplication.Test.Domain.Models;

public class MovieTests
{
    [Fact]
    public void Movie_InitializedWithValidData_ShouldSetPropertiesCorrectly()
    {
        int id = 1;
        string title = "Inception";

        var movie = new Movie(id, title);

        Assert.Equal(id, movie.Id);
        Assert.Equal(title, movie.Title);
    }

    // Immutability Test
    [Fact]
    public void Movie_Properties_ShouldBeReadOnly()
    {
        // Only testing for Id and Title
        Assert.False(typeof(Movie).GetProperty(nameof(Movie.Id))?.CanWrite);
        Assert.False(typeof(Movie).GetProperty(nameof(Movie.Title))?.CanWrite);
    }
}
