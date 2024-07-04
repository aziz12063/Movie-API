using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;

namespace ApiApplication.Profiles
{
    public class SeatProfile : Profile
    {
        
        public SeatProfile()
        {
            // we create a map from Seat Entity to SeatDto
            CreateMap<SeatEntity, SeatDto>().ForMember(dest => dest.Auditorium, opt => opt.MapFrom(src => src.Auditorium));//.ForMember(dest => dest.seatId, opt => opt.Ignore());
                                            
            CreateMap<SeatDto, SeatEntity>().ForMember(dest => dest.Auditorium, opt => opt.MapFrom(src => src.Auditorium));//.ForMember(dest => dest.Auditorium, opt => opt.Ignore());
           
        }
    }
}
