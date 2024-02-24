using System;

namespace ApiApplication.Domain.Models
{
    public class Movie
    {
        public int Id { get; }
        public string Title { get; }
        public string ImdbId { get; }
        public string Stars { get; }
        public DateTime ReleaseDate { get; }

        public Movie(int id, string title, string imdbId, string stars, DateTime releaseDate)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be positive.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or whitespace.");

            Id = id;
            Title = title;
            ImdbId = imdbId;
            Stars = stars;
            ReleaseDate = releaseDate;
        }

        public Movie(int id, string title) : this(id, title, null, null, DateTime.MinValue)
        {
        }

    }
}
