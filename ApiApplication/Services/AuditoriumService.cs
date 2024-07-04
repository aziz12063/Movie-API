using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _auditoriumsRepository;

        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository) 
        { 
            _auditoriumsRepository = auditoriumsRepository;
        }


        public async Task<bool> AuditoriumExistAsync(int auditoriumId)
        {
            try
            {
                return await _auditoriumsRepository.AuditoriumExistAsync(auditoriumId);
            }
            catch(Exception  ex) 
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
