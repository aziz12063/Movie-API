using ApiApplication.Database;
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
        public List<AuditoriumDto> auditoriumDtos = new List<AuditoriumDto>();

        public AuditoriumService(CinemaContext dbContext, IMapper mapper) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }



        public async Task<List<AuditoriumDto>> GetAuditoriums()
        {

            var auditoriums = await  _dbContext.Auditoriums.ToListAsync();
            
            foreach (var auditorium in auditoriums)
            {
                 auditoriumDtos.Add(_mapper.Map<AuditoriumDto>(auditorium));
            }

            return auditoriumDtos;
        }

        public async Task<AuditoriumDto> GetAuditorium(int auditoriumId)
        {
            var auditorium = await _dbContext.Auditoriums.FirstOrDefaultAsync(c => c.auditoriumId == auditoriumId);

            return (_mapper.Map<AuditoriumDto>(auditorium));
        }

        public bool IsTheAuditoriumAvailable(int auditoriumId)
        {
            // implement the logic and modify the return
            return false;
        }
    }
}
