
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using ApiApplication.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("showtimes")]
    public class ShowtimesController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<ShowtimesController> _logger;

        public ShowtimesController(IShowtimeService showtimeService ,ILogger<ShowtimesController> logger) 
        {
            _showtimeService = showtimeService;
            _logger = logger;
        }

        [HttpPost("{movieId}/{sessionDate}/{auditoriumId}")]
        public async Task<ActionResult<ShowtimeDto>> CreateShowtime(string movieId,DateTime sessionDate, int auditoriumId, CancellationToken cancel)
        {
            // session date is like mm-dd-yyy

            if( auditoriumId <= 0)
            {
                _logger.LogError("Invalid auditoriumId {auditoriumId}", auditoriumId);// NB
                return BadRequest($"Invalid auditoriumId {auditoriumId}");
            }
            if(sessionDate <= DateTime.Now)
            {
                _logger.LogError($"Invalid sessionDate {sessionDate}");
                return BadRequest($"Invalid sessionDate {sessionDate}");
            }

            ShowtimeDto showtime = new()
            {
                Movie = new MovieDto() { Id = movieId},
                SessionDate = sessionDate,
                AuditoriumId = auditoriumId,

            };

            if(!await _showtimeService.CreateShowTime(showtime, cancel))
            {
                _logger.LogError("the showtime can not be created, some item are null");
                return NotFound();
            }

            return Ok(showtime);
        }
        /*
        // i will modify this method, i need the id of the movie to grab
        [HttpGet("movie-api/{id}")]
        public async Task<ActionResult<MovieDto>> GetMovieFromProvidedApi(string id)
        {
            Console.WriteLine("beging of GetMovieFromProvidedApi");
            MovieDto movie = await _movieService.ConvertTaskToObject(id);

            if(movie == null)
            {
                Console.WriteLine("no movie found");
                return null;
            }
            Console.WriteLine("befor write the movie in the end of GetMovieFromProvidedApi");
            x = movie.Id;
            Console.WriteLine(x);
            Console.WriteLine(movie.Title);

            return Ok(movie);
        }

        // to delete
        //[HttpGet]
        //public async Task<ActionResult<ICollection<Movie>>> CreateMovie(int id)
        //{
        //    MovieService movieService = new MovieService();
        //    var movies =  movieService.GenerateMovies();
        //    return movies;
        //}
        
        */

    }
}
