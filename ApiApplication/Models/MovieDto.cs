using System;

namespace ApiApplication.Models
{
    public class MovieDto
    {
        // some Movie Ids: tt0111161
        // tt0068646 tt0468569  tt04468569

        // all those  properties will be after that readOnly, i make it public set befor using the provided API
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }

        public MovieDto(string id, string title, string imdbId, string stars, DateTime releaseDate)
        {
            
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or whitespace.");

            Id = id;
            Title = title;
            ImdbId = imdbId;
            Stars = stars;
            ReleaseDate = releaseDate;
        }

        public MovieDto(string id, string title) : this(id, title, null, null, DateTime.MinValue)
        {
        }

        public MovieDto()
        {

        }
    }
}
