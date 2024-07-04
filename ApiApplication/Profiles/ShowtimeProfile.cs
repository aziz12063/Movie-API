using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;


namespace ApiApplication.Profiles
{
    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile() 
        {
            // we create a map from ShitimeEntity to ShowtimeDto
            CreateMap<ShowtimeEntity, ShowtimeDto>().ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets))
                                                    .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.Movie));

            CreateMap<ShowtimeDto, ShowtimeEntity>().ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets))
                                                    .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.Movie));

        }
    }
}
