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
           

        //private async Task<TicketDto> CreateTicketDtoAsync(IEnumerable<SeatDto> availableSeatsDto, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto, CancellationToken cancel)
        //{
        //    Guid guid = Guid.NewGuid();

           

        //    var seatsToReserve = await _seatService.FindSeatsContiguous(availableSeatsDto, nbrOfSeatsToReserve, showtimeDto, cancel);


            //try
            //{

            //    // make the seats IsRreserved
            //    seatsToReserve = await _seatService.UpdateSeatsState(seatsToReserve);

            //    TicketDto ticketDto = new()
            //    {
            //        TicketId = guid,
            //        ShowtimeId = showtimeDto.showtimeId,
            //        Seats = seatsToReserve,
            //        CreatedTime = DateTime.Now,
            //        Paid = false,
            //        //Showtime = showtimeDto,
        //        };

        //        showtimeDto.Tickets.Add(ticketDto);

        //        _tickets.Add(guid, ticketDto);

        //        Timer timer = new Timer(HandleCancellation, guid, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);

        //        return ticketDto;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
   
        //}


        private async Task<TicketDto> CreateTicketDtoAsync(ShowtimeDto showtimeDto, CancellationToken cancel)
        {
            Guid guid = Guid.NewGuid();

            try
            {

                TicketDto ticketDto = new()
                {
                    TicketId = guid,
                    ShowtimeId = showtimeDto.showtimeId,
                 
                    CreatedTime = DateTime.Now,
                    Paid = false,
                    
                };

                showtimeDto.Tickets.Add(ticketDto);
                // i should update the showtimeentity including this ticket to tickets

                _tickets.Add(guid, ticketDto);

                Timer timer = new Timer(HandleCancellation, guid, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);

                return ticketDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

     
        public async Task<TicketDto> CreateTicketWithDelayAsync(TicketDto ticketDto, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            // i should first check if the showtime exist
            // 

            var showtimeEntityWithTickets = await _showtimesRepository.GetWithAuditAndTicketsAndSeats(ticketDto.ShowtimeId, cancel);

            var availableSeatsEntity = showtimeEntityWithTickets.Auditorium.Seats.Where(s => s.IsReserved == false).ToList();

            Guid guid = Guid.NewGuid();

            var seatsToReserve = await _seatService.FindSeatsContiguous(availableSeatsEntity, nbrOfSeatsToReserve, cancel);

            try
            {
                seatsToReserve = await _seatService.UpdateSeatsState(seatsToReserve);

                TicketEntity ticketEntity = new()
                {
                    TicketId = guid,
                    ShowtimeId = ticketDto.ShowtimeId,
                    Seats = seatsToReserve,
                    CreatedTime = DateTime.Now,
                    Paid = false,
                    Showtime = showtimeEntityWithTickets
                };

                showtimeEntityWithTickets.Tickets.Add(ticketEntity);
                ticketDto.CreatedTime = DateTime.Now;

                _tickets.Add(guid, ticketDto);

                Timer timer = new Timer(HandleCancellation, guid, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);

                var createdTicket = await _ticketsRepository.CreateAsync(ticketEntity, cancel);

                return _mapper.Map<TicketDto>(createdTicket);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


   
        }

        // To do: logic to cancel the ticket
        private  void HandleCancellation(object state)
        {
            Guid guid = (Guid)state;

            
            if ((_tickets.TryGetValue(guid, out TicketDto ticketDto)))
            {
                
                // Check if seats are paid after 10 minutes
                if (!ticketDto.Paid)
                {
                    TicketEntity ticket = _mapper.Map<TicketEntity>(ticketDto);
                    // i update seats to not reserved
                    _seatService.UpdateSeatsState(ticket.Seats.ToList());

                    
                    // this remove ticket from dic, handle other remove scenario
                    RemoveTicket(guid);

                    // modify other properties if needed

                    _logger.LogInformation("Reservation {ReservationId} canceled because seats were not paid.", ticketDto.TicketId);
                }
            }
        }

        private void RemoveTicket(Guid guid)
        {
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

           

            ChangeBoolState(ticketEntity.Paid);

            await _ticketsRepository.ConfirmPaymentAsync(ticketEntity, cancellation);
            
            return;
        }
    }
}
