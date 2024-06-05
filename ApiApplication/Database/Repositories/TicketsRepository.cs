using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using ApiApplication.Database.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using ApiApplication.Services.Interfaces;

namespace ApiApplication.Database.Repositories
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly CinemaContext _context;
        private readonly ILogger<TicketsRepository> _logger;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly ISeatService _seatService;

        public TicketsRepository(CinemaContext context, IShowtimesRepository showtimesRepository, ILogger<TicketsRepository> logger, ISeatService seatService)
        {
            _context = context;
            _logger = logger;
            _showtimesRepository = showtimesRepository;
            _seatService = seatService;
        }

        public Task<TicketEntity> GetByIdAsync(Guid id, CancellationToken cancel)
        {
            return _context.Tickets.Include(t => t.Seats).FirstOrDefaultAsync(x => x.TicketId == id, cancel);
        }

        public async Task<TicketEntity> CreateAsync(TicketEntity ticketEntity, CancellationToken cancel)
        {


            var existingSeats = ticketEntity.Seats.ToList();
            List<SeatEntity> seats = new();

            foreach (var seat in existingSeats)
            {
                var existingSeat = _context.ChangeTracker.Entries<SeatEntity>()
                    .FirstOrDefault(e => e.Entity.AuditoriumId == seat.AuditoriumId && e.Entity.Row == seat.Row && e.Entity.SeatNumber == seat.SeatNumber)?.Entity;

                if (existingSeat != null)
                {
                    //_context.ChangeTracker.Entries<SeatEntity>();
                    _context.Entry(existingSeat).State = EntityState.Modified;

                    seats.Add(existingSeat);
                }
                else
                {
                   
                    
                }
            }

            ticketEntity.Seats.Clear();
            ticketEntity.Seats=seats;

            var ticket = _context.Tickets.Add(ticketEntity);

            await _context.SaveChangesAsync(cancel);
            return ticket.Entity;
        }


        public async Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel)
        {
            _logger.LogInformation("in ticketRepo, ConfirmationPayment");
            ticket.Paid = true;
            _context.Update(ticket);
            await _context.SaveChangesAsync(cancel);
            return ticket;
        }

       
    }
}
