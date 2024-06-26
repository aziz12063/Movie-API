using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        //private readonly CinemaContext _dbContext;
        //private readonly IMapper _mapper;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        //public List<AuditoriumDto> auditoriumDtos = new List<AuditoriumDto>();

        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository) 
        {
            //_dbContext = dbContext;
            _auditoriumsRepository = auditoriumsRepository;
            //_mapper = mapper;
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
