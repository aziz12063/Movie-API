using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class SeatService : ISeatService
    {
        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        public List<SeatDto> seats = new List<SeatDto>();
        public SeatService(CinemaContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        
        //public async Task<List<SeatDto>> GettSeats(int auditoriumId)
        //{

        //    var auditoriums = await _dbContext.Auditoriums.FirstOrDefaultAsync(c  => c.Id == auditoriumId);

        //    var auditoriumDto = _mapper.Map<AuditoriumDto>(auditoriums);

        //    seats =  auditoriumDto.Seats.ToList();

            

        //    return seats;
        //}

      




    }
}
