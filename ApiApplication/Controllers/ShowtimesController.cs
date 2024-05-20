
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using ApiApplication.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/showtimes")]
    public class ShowtimesController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<ShowtimesController> _logger;

        public ShowtimesController(IShowtimeService showtimeService ,ILogger<ShowtimesController> logger) 
        {
            _showtimeService = showtimeService;
            _logger = logger;
        }


        //[Route("./{movieId}/{sessionDate}/{auditoriumId")]
        [HttpPost("{movieId}/{sessionDate:CustomDate}/{auditoriumId:int}", Name = "CreateShowtime")]
        public async Task<ActionResult<ShowtimeDto>> CreateShowtime(string movieId, int auditoriumId, DateTime sessionDate, CancellationToken cancel)
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
                sessionDate = DateTime.Now.AddDays(5);
                // i do this just for dev
                return BadRequest($"Invalid sessionDate {sessionDate}");
            }

            //ActionResult<ShowtimeDto> showtimeDto = await _showtimeService.CreateShowTime(movieId, auditoriumId, sessionDate, cancel);
            var showtime =  await _showtimeService.CreateShowTime(movieId, auditoriumId, sessionDate, cancel);


            if (showtime == null)
            {
                _logger.LogError("the showtime can not be created, some item are null");
                // modify this return
                return NotFound();
            }

            // get the showtime just created:
            ShowtimeDto createdShowtimeDto = await _showtimeService.GetShowtimeByAuditoriumIdAndSessionDate(auditoriumId, sessionDate, cancel);
            if (createdShowtimeDto == null)
            {
                return NotFound();
            }

            int x = createdShowtimeDto.Id;
            return Ok(createdShowtimeDto);

            // not work try again
            /*
            return RedirectToRoute("GetShowtimeWithMovie",
                    new
                    {
                        id = x //showtime.Value.Id
                        //movieId = showtime.Value.Movie.Id,
                        //sessionDate = showtime.Value.SessionDate.ToString("MM-dd-yyyy"),
                        //auditoriumId = showtime.Value.AuditoriumId

                    }
                    //createdShowtimeDto
                    );*/
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
