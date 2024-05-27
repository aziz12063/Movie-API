using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;

namespace ApiApplication.Profiles
{
    public class AuditoriumProfile : Profile
    {
        public AuditoriumProfile()
        {
            CreateMap<AuditoriumEntity, AuditoriumDto>().ForMember(dest => dest.Showtimes, opt => opt.MapFrom(src => src.Showtimes))
                                                        .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));
            CreateMap<AuditoriumDto, AuditoriumEntity>().ForMember(dest => dest.Showtimes, opt => opt.MapFrom(src => src.Showtimes))
                                                        .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));
        }
    }
}
