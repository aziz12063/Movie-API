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

namespace ApiApplication.Database.Repositories
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly CinemaContext _context;
        private readonly ILogger<TicketsRepository> _logger;
        private readonly IShowtimesRepository _showtimesRepository;

        public TicketsRepository(CinemaContext context, IShowtimesRepository showtimesRepository, ILogger<TicketsRepository> logger)
        {
            _context = context;
            _logger = logger;
            _showtimesRepository = showtimesRepository;
        }

        public Task<TicketEntity> GetByIdAsync(Guid id, CancellationToken cancel)
        {
            return _context.Tickets.FirstOrDefaultAsync(x => x.ticketId == id, cancel);
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

            //foreach (var seat in ticketEntity.Seats)
            //{

            //    _context.Entry(seat).State = EntityState.Detached;

            //}
            //_context.ChangeTracker.Entries().ToList().ForEach(entry => entry.State = EntityState.Detached);


            // the error is here when trying to add ticket

            //using (var context = new CinemaContext(new DbContextOptionsBuilder<CinemaContext>()
            //                                            .UseInMemoryDatabase("CinemaDb")
            //                                            .Options)) 
            //{
            //    var ticket = _context.Tickets.Add(ticketEntity);

            //    // delete the log
            //    _logger.LogInformation("in TicketsRepository line 69 ");
            //    await _context.SaveChangesAsync(cancel);
            //    // delete the log
            //    _logger.LogInformation("in TicketsRepository line 72 ");
            //    return ticket.Entity;

            //}
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

        public async Task <TicketEntity> UpdateTicketEntity(Guid guid, int showtimeId, CancellationToken cancel)
        {
            var ticket = await _context.Tickets.Include(t => t.Seats)
                                                
                                                .FirstOrDefaultAsync(t => t.ticketId == guid);

            

            ticket.Seats = 
           
            await _context.SaveChangesAsync(cancel);

            return ticket;
        }
    }
}
