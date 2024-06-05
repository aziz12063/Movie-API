using ApiApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Services.Interfaces
{
    public interface IAuditoriumService
    {
       Task<bool> AuditoriumExistAsync(int auditoriumId);
    }
}
