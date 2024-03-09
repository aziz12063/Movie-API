using ApiApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Services.Interfaces
{
    public interface IAuditoriumService
    {
       bool IsTheAuditoriumAvailable(int auditoriumId);
        Task<AuditoriumDto> GetAuditorium(int auditoriumId);
        Task<List<AuditoriumDto>> GetAuditoriums();
    }
}
