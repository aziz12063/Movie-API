using ApiApplication.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/Tickets")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }


        //[Route("./{movieId}/{sessionDate}/{auditoriumId")]
        [HttpPost("{showtimeId:int}/{nbrOfSeats:int}", Name = "CreateTicket")]
        public async Task<ActionResult<TicketDto>> CreateTicket(int showtimeId, int nbrOfSeats, CancellationToken cancel)
        {
            //check for the input
            if(showtimeId <= 0)
            {
                _logger.LogError("showtime Id {showtimeId} is not valid", showtimeId);
                return BadRequest();
            }
            
            if(nbrOfSeats <= 0)
            {
                _logger.LogError(" the nombre of seat to reserve {nbrOgSeats} is not valid", nbrOfSeats);
                return BadRequest();
            }

            var ticketDto = await _ticketService.CreateTicketWithDelayAsync(showtimeId, nbrOfSeats, cancel);

            if (ticketDto == null)
            {
                _logger.LogError("some error occur when creating ticket");
                return BadRequest();
            }

            return Ok(ticketDto);
        }

        [HttpPost("{guid:Guid}", Name = "BayTicket")]
        public async Task<ActionResult<TicketDto>> BayTicket(string guid, CancellationToken cancel)
        {
            if (!Guid.TryParse(guid, out Guid result ))
            {
                throw new FormatException($"invalid forrmat of the guid {guid}");
            }
            await _ticketService.ConfirmPayementAsync(result, cancel);

            return Ok();
        }

    }
}
