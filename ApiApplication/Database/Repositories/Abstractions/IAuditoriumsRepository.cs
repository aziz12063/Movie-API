using ApiApplication.Database.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Database.Repositories.Abstractions
{
    public interface IAuditoriumsRepository
    {
        Task<AuditoriumEntity> GetAsync(int auditoriumId, CancellationToken cancel);
        bool IsTheAuditoriumAvailable(int auditoriumId, DateTime sessionDate);
        Task<AuditoriumEntity> GetByIdIncludShowtimeAsync(int auditoriumId, CancellationToken cancel);
    }
}