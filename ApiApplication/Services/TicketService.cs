using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class TicketService : ITicketService
    {

        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISeatService _seatService;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly ILogger<TicketService> _logger;
        private readonly ITicketsRepository _ticketsRepository;

        //List<SeatDto> listOfSeatsreserved = new List<SeatDto>();

        public TicketService(CinemaContext dbContext,
                             IMapper mapper,
                             IShowtimesRepository showtimesRepository,
                             ISeatService seatService,
                             ILogger<TicketService> logger,
                             ITicketsRepository ticketsRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _seatService = seatService;
            _showtimesRepository = showtimesRepository;
            _logger = logger;
            _ticketsRepository = ticketsRepository;
        }
           

        private async Task<TicketDto> CreateTicketDtoAsync(int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            Guid guid = Guid.NewGuid();

            // I grab the ShowTime:
            var showtime = await _showtimesRepository.GetWithSeatsByIdAsync(showtimeId, cancel);
            if (showtime == null)
            {
                _logger.LogError("no showtime found");

                return null;
            }

            ShowtimeDto showtimeDto = _mapper.Map<ShowtimeDto>(showtime);

            // get the auditoriumId:
            int auditoriumId = showtimeDto.AuditoriumId;

            // Find Seats Contiguous:
            var listSeatsToReserve = await _seatService.FindSeatsContiguous(auditoriumId, nbrOfSeatsToReserve, showtimeDto);

            if (listSeatsToReserve == null || listSeatsToReserve.Count == 0)
            {
                _logger.LogError("no seat to reserve");
                return null;
            }

            // here the seats are reserved
            listSeatsToReserve = await _seatService.UpdateSeatsState(listSeatsToReserve);
            TicketDto ticketDto = new()
            {
                Id = guid,
                ShowtimeId = showtimeId,
                Seats = listSeatsToReserve,
                CreatedTime = DateTime.Now,
                Paid = false,
                Showtime = showtimeDto

            };
            
            return ticketDto;
        }

        public async Task CreateTicketWithDelayAsync(int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            var ticketDto = await CreateTicketDtoAsync(showtimeId, nbrOfSeatsToReserve, cancel);


            if (ticketDto == null)
            {
                _logger.LogError("the ticket is null");
                return;

            }

            // Start the delay task with the provided CancellationToken
            var delayTask = Task.Delay(TimeSpan.FromMinutes(10), cancel);

            // ContinueWith the cancellation logic
            await delayTask.ContinueWith(t =>
            {
                HandleCancellation(ticketDto);
            });


            return;
        }

        // To do: logic to cancel the ticket
        private  void HandleCancellation(TicketDto reservationDto)
        {
          
            // Check if seats are paid after 10 minutes
            if (!reservationDto.Paid)
            {
                // i update seats to not reserved
                _seatService.UpdateSeatsState(reservationDto.Seats.ToList());

                //i delete the ticketDto
                reservationDto = null;
                              
                _logger.LogInformation("Reservation {ReservationId} canceled because seats were not paid.", reservationDto.Id);
            }

        }

        private void ChangeTicketState(TicketDto ticketDto)
        {
            ticketDto.Paid = !ticketDto.Paid;
        }

        
        

        public async Task ConfirmPayementAsync(Guid id, CancellationToken cancellation)
        {


            TicketEntity ticketEntity = await _ticketsRepository.GetAsync(id, cancellation);
            TicketDto ticketDto = new TicketDto();

            if (ticketEntity == null)
            {
                _logger.LogError("no ticket found");
                
            }

            ticketDto = _mapper.Map<TicketDto>(ticketEntity);

            // check if the reservation is still alive
            // ***************************i will modify a property: Isexpired**********************************
            if (ticketDto.Isexpired)
            {
                _logger.LogError("the reservation is expired");
            }

            ChangeTicketState(ticketDto);

            // call ConfirmPayementAsync from TicketRepo

            return;
            
        }
        
    }
}
