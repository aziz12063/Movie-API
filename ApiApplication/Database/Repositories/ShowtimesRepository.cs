using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Linq.Expressions;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using AutoMapper;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Database.Repositories
{
    public class ShowtimesRepository : IShowtimesRepository
    {
        private readonly CinemaContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimesRepository> _logger;

        public ShowtimesRepository(CinemaContext context, IMapper mapper, ILogger<ShowtimesRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShowtimeEntity> GetWithMoviesByIdAsync(int id, CancellationToken cancel)
        {
            try
            {
                return await _context.Showtimes
                .Include(x => x.Movie)
                .FirstOrDefaultAsync(x => x.showtimeId == id, cancel);
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
                .FirstOrDefaultAsync(x => x.showtimeId == id, cancel);
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
                .FirstOrDefaultAsync(x => x.showtimeId == id, cancel);
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

       

        public async Task<ShowtimeDto> CreateShowtime(ShowtimeEntity showtimeEntity, CancellationToken cancel)
        {

            var showtime = await _context.Showtimes.AddAsync(showtimeEntity, cancel);
            await _context.SaveChangesAsync(cancel);
            ShowtimeDto showtimeDto = _mapper.Map<ShowtimeDto>(showtime);

            // log the showtimeDto
            // delete it after
            var properties = showtimeDto.Movie.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value = property.GetValue(showtimeDto.Movie);
                _logger.LogInformation($"{property.Name}: {value}");
            }

            return showtimeDto;
        }
    }
}
