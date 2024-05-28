using ApiApplication.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using ApiApplication.Database.Repositories.Abstractions;
using AutoMapper;
using ApiApplication.Database.Repositories;
using System.Linq;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/Tickets")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMapper _mapper;
        private readonly IAuditoriumsRepository _auditoriumsRepository;

        public TicketsController(ITicketService ticketService,
                                ILogger<TicketsController> logger,
                                IShowtimesRepository showtimesRepository,
                                IMapper mapper,
                                IAuditoriumsRepository auditoriumsRepository)
        {
            _ticketService = ticketService;
            _logger = logger;
            _showtimesRepository = showtimesRepository;
            _mapper = mapper;
            _auditoriumsRepository = auditoriumsRepository;
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

            var showtimeEntityWithTickets = await _showtimesRepository.GetWithTicketsByIdAsync(showtimeId, cancel);

            var showtimeDtoWithTickets = _mapper.Map<ShowtimeDto>(showtimeEntityWithTickets);


            var auditoriumEntityWithSeats = await _auditoriumsRepository.GetByIdWithSeatsAsync(showtimeId, cancel);


            var auditoriumDtoWithSeats = _mapper.Map<AuditoriumDto>(auditoriumEntityWithSeats);

            var allSeatsDto = auditoriumDtoWithSeats.Seats.ToList();

            var reservedSeatsDto = showtimeDtoWithTickets.Tickets.SelectMany(t => t.Seats).ToList();

            var availableSeatsDto = allSeatsDto.Except(reservedSeatsDto);

            var showtimeDto = _mapper.Map<ShowtimeDto>(await _showtimesRepository.GetByIdAsync(showtimeId, cancel));

            var ticketDto = await _ticketService.CreateTicketWithDelayAsync(availableSeatsDto, nbrOfSeats, showtimeDto, cancel);




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
