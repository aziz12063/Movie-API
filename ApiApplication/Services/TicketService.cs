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

        private Dictionary<Guid, TicketDto> _tickets = new();
        private Dictionary<Guid, Timer> _timers = new();

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
           

        private async Task<TicketDto> CreateTicketDtoAsync(IEnumerable<SeatDto> availableSeatsDto, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto, CancellationToken cancel)
        {
            Guid guid = Guid.NewGuid();

            var seatsToReserve = await _seatService.FindSeatsContiguous(availableSeatsDto, nbrOfSeatsToReserve, showtimeDto, cancel);


            try
            {

                // make the seats IsRreserved
                seatsToReserve = await _seatService.UpdateSeatsState(seatsToReserve);

                TicketDto ticketDto = new()
                {
                    ticketId = guid,
                    ShowtimeId = showtimeDto.showtimeId,
                    //Seats = seatsToReserve,
                    CreatedTime = DateTime.Now,
                    Paid = false,
                    //Showtime = showtimeDto,
                };

                showtimeDto.Tickets.Add(ticketDto);

                _tickets.Add(guid, ticketDto);

                Timer timer = new Timer(HandleCancellation, guid, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);

                return ticketDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
   
        }

        // call this from the controller
        public async Task<TicketDto> CreateTicketWithDelayAsync(IEnumerable<SeatDto> availableSeatsDto, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto, CancellationToken cancel)
        {
            // i should first check if the showtime exist
            // 

            
            TicketDto ticketDto = await CreateTicketDtoAsync(availableSeatsDto, nbrOfSeatsToReserve, showtimeDto, cancel);

            
            if (ticketDto == null)
            {
                _logger.LogError("the ticket is null");
                return null;
            }


            // i should not mapp here, in the controller
          
            TicketEntity ticketEntity = _mapper.Map<TicketEntity>(ticketDto);


            

            // save the ticket in the DB, this return ticketEntity
            await _ticketsRepository.CreateAsync(ticketEntity, cancel);
            // delete the log
            _logger.LogInformation("in CreateTicketWithDelayAsync line 111 ");

            return ticketDto;
        }

        // To do: logic to cancel the ticket
        private  void HandleCancellation(object state)
        {
            Guid guid = (Guid)state;

            // delete the log
            _logger.LogInformation("in HandleCancellation line 123 ");
            if ((_tickets.TryGetValue(guid, out TicketDto ticketDto)))
            {
                // delete the log
                _logger.LogInformation("in HandleCancellation line 127 ");
                // Check if seats are paid after 10 minutes
                if (!ticketDto.Paid)
                {
                    // delete the log
                    _logger.LogInformation("in HandleCancellation line 132 ");

                    // delete the log
                    _logger.LogInformation("in HandleCancellation line 135 ");
                    // i update seats to not reserved
                    _seatService.UpdateSeatsState(ticketDto.Seats.ToList());

                    // delete the log
                    _logger.LogInformation("in HandleCancellation line 140 ");
                    // this remove ticket from dic, handle other remove scenario
                    RemoveTicket(guid);

                    // modify other properties if needed

                    _logger.LogInformation("Reservation {ReservationId} canceled because seats were not paid.", ticketDto.ticketId);
                }
            }
        }

        private void RemoveTicket(Guid guid)
        {
            // delete the log
            _logger.LogInformation("in RemoveTicket line 154 ");
            _tickets.Remove(guid);
            _timers[guid].Dispose();
            _timers.Remove(guid);
        }

        private void ChangeBoolState(bool property)
        {
            // delete the log
            _logger.LogInformation("in ChangeBoolState line 163 ");
            property = !property;
        }

        
        
        // change the return on this method
        public async Task ConfirmPayementAsync(Guid id, CancellationToken cancellation)
        {
            // to do

            // i add it to ShowtimeEntity
            // i add the showtime to auditoriumEntity
            // i add the shotimes to the movie entity

            TicketEntity ticketEntity = await _ticketsRepository.GetByIdAsync(id, cancellation);

            if (ticketEntity == null)
            {
                _logger.LogError("no ticket found");
                throw new ArgumentNullException(nameof(ticketEntity));
            }

            TicketDto ticketDto = _mapper.Map<TicketDto>(ticketEntity);

           
            // check if the reservation is still alive
            // ***************************i will modify a property: Isexpired**********************************
            //if (ticketDto.IsExpired)
            //{
            //    _logger.LogError("the reservation is expired");
            //    return;
            //}

            ChangeBoolState(ticketDto.Paid);


            ticketEntity = _mapper.Map<TicketEntity>(ticketDto);

            await _ticketsRepository.ConfirmPaymentAsync(ticketEntity, cancellation);
            
            return;
        }
    }
}
