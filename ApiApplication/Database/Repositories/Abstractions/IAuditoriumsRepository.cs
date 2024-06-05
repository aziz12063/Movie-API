using ApiApplication.Database.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Database.Repositories.Abstractions
{
    public interface IAuditoriumsRepository
    {
        Task<AuditoriumEntity> GetByIdWithSeatsAndShowtimesAsync(int auditoriumId, CancellationToken cancel);
        bool IsTheAuditoriumAvailable(int auditoriumId, DateTime sessionDate);
        Task<AuditoriumEntity> GetByIdIncludShowtimeAsync(int auditoriumId, CancellationToken cancel);
        Task<bool> AuditoriumExistAsync(int auditoriumId);
    }
}