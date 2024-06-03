using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;


        //private Dictionary<Guid, TicketDto> _tickets = new();
        private Dictionary<Guid, Timer> _timers = new();

        private const string TicketsCachKey = "Tickets";

        public TicketService(CinemaContext dbContext,
                             IMapper mapper,
                             IShowtimesRepository showtimesRepository,
                             ISeatService seatService,
                             ILogger<TicketService> logger,
                             ITicketsRepository ticketsRepository,
                             IMemoryCache cache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _seatService = seatService;
            _showtimesRepository = showtimesRepository;
            _logger = logger;
            _ticketsRepository = ticketsRepository;
            _cache = cache;
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


                Dictionary<Guid, TicketDto> _tickets = GetTicketsFromCache();

                _tickets.Add(guid, ticketDto);

                SetTicketInCache(_tickets);

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

            _logger.LogInformation("in ticketService, HandleCancellation 1");

            Dictionary<Guid, TicketDto> _tickets = GetTicketsFromCache();

            if ((_tickets.TryGetValue(guid, out TicketDto ticketDto)))
            {
                // Check if seats are paid after 10 minutes
                if (!ticketDto.Paid)
                {
                    TicketEntity ticket = _mapper.Map<TicketEntity>(ticketDto);

                    // i update seats to not reserved
                    _seatService.UpdateSeatsState(ticket.Seats.ToList());

                    
                    // this remove ticket from dic, handle other remove scenario
                    RemoveTicket(guid, _tickets);
                    SetTicketInCache(_tickets);

                    _logger.LogInformation("Reservation {ReservationId} canceled because seats were not paid.", ticketDto.TicketId);
                }
            }
        }

        private void RemoveTicket(Guid guid, Dictionary<Guid, TicketDto> _tickets)
        {
            _tickets.Remove(guid);
            _timers[guid].Dispose();
            _timers.Remove(guid);
        }

        
        // change the return on this method
        public async Task<bool> ConfirmPayementAsync(Guid id, CancellationToken cancellation)
        {
            Dictionary<Guid, TicketDto> _tickets = GetTicketsFromCache();


            TicketEntity ticketEntity;
            // icheck if ticket is still alive
            
           
            bool exist = _tickets.ContainsKey(id);
            if (exist)
            {
                _logger.LogInformation(" the guid exist from ConfirmationPayment");
            }
            else
            {
                _logger.LogInformation(" the guid does not exist from ConfirmationPayment");
                return false;
            }
            _logger.LogInformation("i checked that the seats are reserved");
            ticketEntity = await _ticketsRepository.GetByIdAsync(id, cancellation);
            if (ticketEntity != null)
            {
                _logger.LogInformation("The ticket is found in the dic");
                if(ticketEntity.Seats.All(seat => seat.IsReserved))
                {
                    await _ticketsRepository.ConfirmPaymentAsync(ticketEntity, cancellation);
                    _logger.LogInformation("Payed");
                    return true;
                }
            }
            return false;
        }

        private Dictionary<Guid, TicketDto> GetTicketsFromCache()
        {
            return _cache.Get<Dictionary<Guid, TicketDto>>(TicketsCachKey) ?? new Dictionary<Guid, TicketDto>();
        }

        private void SetTicketInCache(Dictionary<Guid, TicketDto> tickets)
        {
            _cache.Set(TicketsCachKey, tickets);
        }
    }
}
