using ApiApplication.Database;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        public List<AuditoriumDto> auditoriumDtos = new List<AuditoriumDto>();

        public AuditoriumService(CinemaContext dbContext, IMapper mapper, IAuditoriumsRepository auditoriumsRepository) 
        {
            _dbContext = dbContext;
            _auditoriumsRepository = auditoriumsRepository;
            _mapper = mapper;
        }


        // put this method in the repo and call it
        public async Task<List<AuditoriumDto>> GetAuditoriums()
        {

            var auditoriums = await  _dbContext.Auditoriums.ToListAsync();
            
            foreach (var auditorium in auditoriums)
            {
                 auditoriumDtos.Add(_mapper.Map<AuditoriumDto>(auditorium));
            }

            return auditoriumDtos;
        }

        // put this method in the repo and call it
        public async Task<AuditoriumDto> GetAuditorium(int auditoriumId)
        {
            var auditorium = await _dbContext.Auditoriums.FirstOrDefaultAsync(c => c.auditoriumId == auditoriumId);

            return (_mapper.Map<AuditoriumDto>(auditorium));
        }

        public async Task<bool> AuditoriumExistAsync(int auditoriumId)
        {
            return await _auditoriumsRepository.AuditoriumExestAsync(auditoriumId);
        }
    }
}
