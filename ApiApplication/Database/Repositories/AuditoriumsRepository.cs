using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using ApiApplication.Database.Repositories.Abstractions;

namespace ApiApplication.Database.Repositories
{
    public class AuditoriumsRepository : IAuditoriumsRepository
    {
        private readonly CinemaContext _context;

        public AuditoriumsRepository(CinemaContext context)
        {
            _context = context;
        }

        public async Task<AuditoriumEntity> GetByIdWithSeatsAndShowtimesAsync(int auditoriumId, CancellationToken cancel)
        {
            return await _context.Auditoriums
                .Include(x => x.Seats)
                .Include(x => x.Showtimes)
                .FirstOrDefaultAsync(x => x.AuditoriumId == auditoriumId, cancel);
        }


        public async Task<bool> AuditoriumExistAsync(int auditoriumId)
        {
            return await _context.Auditoriums
                    .AnyAsync(a => a.AuditoriumId == auditoriumId);
        }
    }
}
