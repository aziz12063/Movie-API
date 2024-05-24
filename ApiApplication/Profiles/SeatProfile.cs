using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;
using System.Collections.Generic;

namespace ApiApplication.Profiles
{
    public class SeatProfile : Profile
    {
        
        public SeatProfile()
        {
            // we create a map from Seat Entity to SeatDto
            CreateMap<SeatEntity, SeatDto>().ForMember(dest => dest.seatId, opt => opt.Ignore());
                                            //.ForMember(dest => dest.IsReserved, opt => opt.Ignore());
            CreateMap<List<SeatEntity>, List<SeatDto>>();

            CreateMap<SeatDto, SeatEntity>();
            CreateMap<List<SeatDto>, List<SeatEntity>>();
        }
    }
}
