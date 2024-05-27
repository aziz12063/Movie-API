
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using ApiApplication.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using ApiApplication.CustomExceptions;
using ApiApplication.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;
using ApiApplication.Services.Interfaces;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/showtimes")]
    public class ShowtimesController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        private readonly IMovieService _movieService;
        private readonly IAuditoriumService _auditoriumService;

        private readonly ILogger<ShowtimesController> _logger;

        public ShowtimesController(IShowtimeService showtimeService, IAuditoriumService auditoriumService, IMovieService movieService, ILogger<ShowtimesController> logger) 
        {
            _showtimeService = showtimeService;
            _movieService = movieService;
            _auditoriumService = auditoriumService;

            _logger = logger;
        }


        //[Route("./{movieId}/{sessionDate}/{auditoriumId")]
        [HttpPost("{movieId}/{sessionDate:CustomDate}/{auditoriumId:int}", Name = "CreateShowtime")]
        public async Task<ActionResult<ShowtimeDto>> CreateShowtime(string movieId, int auditoriumId, DateTime sessionDate, CancellationToken cancel)
        {
            
            // validate the input values:
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

            // validation of fetching obj with input values

            bool exist = await _auditoriumService.AuditoriumExistAsync(auditoriumId);
            if (!exist)
            {
                _logger.LogError("Invalid auditoriumId {auditoriumId}", auditoriumId);// NB
                return BadRequest($"AuditoriumId with Id: {auditoriumId} could not be found");
            }
            
            if(await _showtimeService.ShowtimeExistAsync(auditoriumId, sessionDate)) 
            {
                _logger.LogError("Canot create the showtime because it's already exist");
                return Conflict("showtimetime already exist");
            }

            MovieDto movie;
            try
            {
                movie = await _movieService.GetMovieById(movieId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while retrieving the movie.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service is unavailable. Please try again later.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "The request timed out while retrieving the movie.");
                return StatusCode(StatusCodes.Status408RequestTimeout, "The request timed out. Please try again.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error processing the movie data.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing the movie data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the movie.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            if (movie == null)
            {
                _logger.LogWarning("The Movie with Id: {movieId} doesn't exist.", movieId);
                throw new InvalidInPutException("the Movie is null");
            }

            ShowtimeDto showtimeDto = new()
            {
                Movie = movie,
                SessionDate = sessionDate,
                AuditoriumId = auditoriumId,
            };

            ShowtimeDto createdShowtimeDto =  await _showtimeService.CreateShowTime(showtimeDto, cancel);

            if (createdShowtimeDto == null)
            {
                _logger.LogError("Cannot save the showtime just created");
                // modify this return
                return StatusCode(500, " Cannot save the showtime just created");
            }
           

            // Generate and write the cURL command to the cUrls.txt file
            string curlCommand = Request.GetDisplayUrl();
            _logger.LogInformation("the curlCommand {curlCommand}", curlCommand);
            await WriteCurlCommandToFile(curlCommand);/////////////////////////////////////

            int x = createdShowtimeDto.showtimeId;
            //return Ok(createdShowtimeDto);

            // not work try again

            return CreatedAtRoute("GetShowtimeWithMovie",
                    new
                    {
                        id = x, //showtime.Value.Id
                                            },
                    createdShowtimeDto
                    );
        }

        // this private method for test to write the curl to file
       
        private async Task WriteCurlCommandToFile(string curlCommand)
        {
            string solutionDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName;
            string filePath = Path.Combine(solutionDirectory, "cUrls.txt");
            await System.IO.File.AppendAllTextAsync(filePath, curlCommand + Environment.NewLine);
        }



        [HttpGet("{id}", Name = "GetShowtimeWithMovie")]
        public async Task<ActionResult<ShowtimeDto>> GetShowtimeWithMovie(int id, CancellationToken cancellation)
        {
            // add some validation

            var showtimeDto = await _showtimeService.GetShowtimeWithMovieById(id, cancellation);
            return Ok(showtimeDto);
        }

        
        [HttpGet("{movieId}/{sessionDate}/{auditoriumId}", Name = "GetShowtimeByAuditoriumIdAndSessionDate")]
        public async Task<ActionResult<ShowtimeDto>> GetShowtimeByAuditoriumIdAndSessionDate(int auditoriumId, DateTime sessionDate, CancellationToken cancel )
        {
            ShowtimeDto createdShowtimeDto = await _showtimeService.GetShowtimeByAuditoriumIdAndSessionDate(auditoriumId, sessionDate, cancel);
            if (createdShowtimeDto == null)
            {
                return NotFound();
            }
            return Ok(createdShowtimeDto);
        }



    }
}
