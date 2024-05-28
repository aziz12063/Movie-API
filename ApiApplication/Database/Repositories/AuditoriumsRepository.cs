using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using ApiApplication.Database.Repositories.Abstractions;
using System.Linq;
using System;
using ApiApplication.Models;

namespace ApiApplication.Database.Repositories
{
    public class AuditoriumsRepository : IAuditoriumsRepository
    {
        private readonly CinemaContext _context;

        public AuditoriumsRepository(CinemaContext context)
        {
            _context = context;
        }

        public async Task<AuditoriumEntity> GetByIdWithSeatsAsync(int auditoriumId, CancellationToken cancel)
        {
            return await _context.Auditoriums
                .Include(x => x.Seats)
                .FirstOrDefaultAsync(x => x.auditoriumId == auditoriumId, cancel);
        }

        public async Task<AuditoriumEntity> GetByIdIncludShowtimeAsync(int auditoriumId, CancellationToken cancel)
        {
            return await _context.Auditoriums
                .Include(x => x.Showtimes)
                .FirstOrDefaultAsync(x => x.auditoriumId == auditoriumId, cancel);
        }


        // delete this mothod
        public bool IsTheAuditoriumAvailable(int auditoriumId, DateTime sessionDate)
        {
            // here i check only if the auditorium exist
            _context.Auditoriums.Any(x => x.auditoriumId == auditoriumId);

            // here i check the availability of the auditorium
            return _context.Auditoriums.Where(auditorium => auditorium.auditoriumId == auditoriumId &&
                                              auditorium.Showtimes.Any(showtime => showtime.SessionDate == sessionDate))
                                       .Any();

        }

        public async Task<bool> AuditoriumExestAsync(int auditoriumId)
        {
            return await _context.Auditoriums
                    .AnyAsync(a => a.auditoriumId == auditoriumId);
        }
    }
}
