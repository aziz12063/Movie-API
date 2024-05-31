using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using ApiApplication.Services.Interfaces;
using ApiApplication.Models;

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
            return _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == id, cancel);
        }

        public async Task<IEnumerable<TicketEntity>> GetByShowtimeIdAsync(int showtimeId, CancellationToken cancel)
        {
            return await _context.Tickets
                .Include(x => x.Showtime)
                .Include(x => x.Seats)
                .Where(x => x.ShowtimeId == showtimeId)
                .ToListAsync(cancel);
        }

        public async Task<TicketEntity> CreateAsync(ShowtimeEntity showtime, IEnumerable<SeatEntity> selectedSeats, CancellationToken cancel)
        {
            var ticket = _context.Tickets.Add(new TicketEntity
            {
                Showtime = showtime,
                Seats = new List<SeatEntity>(selectedSeats)
            });

            await _context.SaveChangesAsync(cancel);

            return ticket.Entity;
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

            // delete the log
            _logger.LogInformation("in TicketsRepository line 69 ");
            await _context.SaveChangesAsync(cancel);
            // delete the log
            _logger.LogInformation("in TicketsRepository line 72 ");
            return ticket.Entity;
        }


        public async Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel)
        {
            ticket.Paid = true;
            _context.Update(ticket);
            await _context.SaveChangesAsync(cancel);
            return ticket;
        }

        public async Task <TicketEntity> UpdateTicketEntity(Guid guid, List<SeatEntity> seats, CancellationToken cancel)
        {
            var ticket = await _context.Tickets.Include(t => t.Seats)
                                                
                                                .FirstOrDefaultAsync(t => t.TicketId == guid);



            ticket.Seats = seats;
           
            await _context.SaveChangesAsync(cancel);

            return ticket;
        }
    }
}
