
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
                return BadRequest($"Invalid sessionDate {sessionDate}");
            }

            //ActionResult<ShowtimeDto> showtimeDto = await _showtimeService.CreateShowTime(movieId, auditoriumId, sessionDate, cancel);
            ActionResult<ShowtimeDto> showtime =  await _showtimeService.CreateShowTime(movieId, auditoriumId, sessionDate, cancel);


            if (showtime == null)
            {
                _logger.LogError("the showtime can not be created, some item are null");
                // modify this return
                return NotFound();
            }

            // get the showtime just created:

            ShowtimeDto createdShowtimeDto = await _showtimeService.GetShowtimeByAuditoriumIdAndSessionDate( auditoriumId, sessionDate, cancel );
            

            return Ok(createdShowtimeDto);
        }
        

    }
}
