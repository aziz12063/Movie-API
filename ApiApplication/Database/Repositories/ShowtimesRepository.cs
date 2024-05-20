using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Linq.Expressions;
using ApiApplication.Database.Repositories.Abstractions;

namespace ApiApplication.Database.Repositories
{
    public class ShowtimesRepository : IShowtimesRepository
    {
        private readonly CinemaContext _context;

        public ShowtimesRepository(CinemaContext context)
        {
            _context = context;
        }

        public async Task<ShowtimeEntity> GetWithMoviesByIdAsync(int id, CancellationToken cancel)
        {
            try
            {
                return await _context.Showtimes
                .Include(x => x.Movie)
                .FirstOrDefaultAsync(x => x.Id == id, cancel);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            
        }

        public async Task<ShowtimeEntity> GetWithSeatsByIdAsync(int id, CancellationToken cancel)
        {
            try
            {
                return await _context.Showtimes
                .Include(x => x.Seats)
                .FirstOrDefaultAsync(x => x.Id == id, cancel);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }


        public async Task<ShowtimeEntity> GetWithTicketsByIdAsync(int id, CancellationToken cancel)
        {
            return await _context.Showtimes
                .Include(x => x.Tickets)
                .FirstOrDefaultAsync(x => x.Id == id, cancel);
        }

        public async Task<IEnumerable<ShowtimeEntity>> GetAllAsync(Expression<Func<ShowtimeEntity, bool>> filter, CancellationToken cancel)
        {
            if (filter == null)
            {
                return await _context.Showtimes
                .Include(x => x.Movie)
                .ToListAsync(cancel);
            }
            return await _context.Showtimes
                .Include(x => x.Movie)
                .Where(filter)
                .ToListAsync(cancel);
        }

        // maube i will delete this method
        public async Task<IEnumerable<ShowtimeEntity>> GetAllByAuditoriumIdAsync(int auditoriumId, CancellationToken cancellation)
        {
            return await _context.Showtimes.Where(showtime => showtime.AuditoriumId == auditoriumId).ToListAsync(cancellation);
        }

        public async Task<ShowtimeEntity> GetByAuditoriumIdAndSessionDateAsync(int auditoriumId,DateTime sessionDate, CancellationToken cancellation)
        {
            try
            {
                return await _context.Showtimes.Where(showtime => showtime.AuditoriumId == auditoriumId || showtime.SessionDate == sessionDate)
                                           .Include(x => x.Movie)
                                           .FirstOrDefaultAsync();
            }
            
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

       

        public async Task<int> CreateShowtime(ShowtimeEntity showtimeEntity, CancellationToken cancel)
        {

            var showtime = await _context.Showtimes.AddAsync(showtimeEntity, cancel);
            var nbrOfMemberSaved = await _context.SaveChangesAsync(cancel);

            return nbrOfMemberSaved;
        }
    }
}
