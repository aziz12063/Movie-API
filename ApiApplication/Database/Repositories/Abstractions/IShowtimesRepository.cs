using ApiApplication.Database.Entities;
using ApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Database.Repositories.Abstractions
{
    public interface IShowtimesRepository
    {
        Task<ShowtimeDto> CreateShowtime(ShowtimeEntity showtimeEntity, CancellationToken cancel);
        Task<IEnumerable<ShowtimeEntity>> GetAllAsync(Expression<Func<ShowtimeEntity, bool>> filter, CancellationToken cancel);
        Task<ShowtimeEntity> GetWithMoviesByIdAsync(int id, CancellationToken cancel);
        Task<ShowtimeEntity> GetWithTicketsByIdAsync(int id, CancellationToken cancel);
        Task<IEnumerable<ShowtimeEntity>> GetAllByAuditoriumIdAsync(int auditoriumId, CancellationToken cancellation);
        Task<ShowtimeEntity> GetWithSeatsByIdAsync(int id, CancellationToken cancel);
        Task<ShowtimeEntity> GetByAuditoriumIdAndSessionDateAsync(int auditoriumId, DateTime sessionDate, CancellationToken cancellation);
    }
}