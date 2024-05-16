using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

            try
            {
                // I grab the ShowTime:
                ShowtimeEntity showtime = await _showtimesRepository.GetWithSeatsByIdAsync(showtimeId, cancel);

                if (showtime == null)
                {
                    _logger.LogError("no showtime found");

                    return null;
                }

                ShowtimeDto showtimeDto = _mapper.Map<ShowtimeDto>(showtime);

                // get the auditoriumId:
                int auditoriumId = showtimeDto.AuditoriumId;

                // Find Seats Contiguous:
                List<SeatDto> listSeatsToReserve = await _seatService.FindSeatsContiguous(auditoriumId, nbrOfSeatsToReserve, showtimeDto);

                if (listSeatsToReserve == null || listSeatsToReserve.Count == 0)
                {
                    _logger.LogError("no seat to reserve");
                    return null;
                }

                // make the seats IsRreserved
                listSeatsToReserve = await _seatService.UpdateSeatsState(listSeatsToReserve);

                TicketDto ticketDto = new()
                {
                    Id = guid,
                    ShowtimeId = showtimeId,
                    Seats = listSeatsToReserve,
                    CreatedTime = DateTime.Now,
                    Paid = false,
                    Showtime = showtimeDto,
                    IsExpired = false

                };

                return ticketDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
   
        }

        // call this from the controller
        public async Task<ActionResult<TicketDto>> CreateTicketWithDelayAsync(int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            TicketDto ticketDto = await CreateTicketDtoAsync(showtimeId, nbrOfSeatsToReserve, cancel);


            if (ticketDto == null)
            {
                _logger.LogError("the ticket is null");
                return null;

            }

 
            TicketEntity ticketEntity = _mapper.Map<TicketEntity>(ticketDto);

            // save the ticket in the DB, this return ticketEntity
            await _ticketsRepository.CreateAsync(ticketEntity, cancel);

            // Start the delay task with the provided CancellationToken
            var delayTask = Task.Delay(TimeSpan.FromMinutes(10), cancel);

            // ContinueWith the cancellation logic
            await delayTask.ContinueWith(t =>
            {
                HandleCancellation(ticketDto);
            });


            return ticketDto;
        }

        // To do: logic to cancel the ticket
        private  void HandleCancellation(TicketDto reservationDto)
        {
          
            // Check if seats are paid after 10 minutes
            if (!reservationDto.Paid)
            {
                // i update seats to not reserved
                _seatService.UpdateSeatsState(reservationDto.Seats.ToList());

                reservationDto.IsExpired = true;
                // modify other properties if needed
                              
                _logger.LogInformation("Reservation {ReservationId} canceled because seats were not paid.", reservationDto.Id);
            }

        }

        private void ChangeBoolState(bool property)
        {
            property = !property;
        }

        
        

        public async Task ConfirmPayementAsync(Guid id, CancellationToken cancellation)
        {
            // to do

            // i add it to ShowtimeEntity
            // i add the showtime to auditoriumEntity
            // i add the shotimes to the movie entity

            TicketEntity ticketEntity = await _ticketsRepository.GetByIdAsync(id, cancellation);
            TicketDto ticketDto = new TicketDto();

            if (ticketEntity == null)
            {
                _logger.LogError("no ticket found");
                
            }

            ticketDto = _mapper.Map<TicketDto>(ticketEntity);

            // check if the reservation is still alive
            // ***************************i will modify a property: Isexpired**********************************
            if (ticketDto.IsExpired)
            {
                _logger.LogError("the reservation is expired");
                return;
            }

            ChangeBoolState(ticketDto.Paid);

            ticketEntity = _mapper.Map<TicketEntity>(ticketDto);

            await _ticketsRepository.ConfirmPaymentAsync(ticketEntity, cancellation);
            
            return;
            
        }
        
    }
}
