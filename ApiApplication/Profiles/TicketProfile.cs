using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;
using System.Collections.Generic;

namespace ApiApplication.Profiles
{
    public class TicketProfile : Profile
    {
        public TicketProfile() 
        {
            CreateMap<TicketEntity, TicketDto>().ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats))
                                                .ForMember(dest => dest.Showtime, opt => opt.MapFrom(src => src.Showtime));
            //CreateMap<List<TicketEntity>, List<TicketDto>>();

            CreateMap<TicketDto, TicketEntity>().ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats))
                                                .ForMember(dest => dest.Showtime, opt => opt.MapFrom(src => src.Showtime));
                                                
            //CreateMap<List<TicketDto>, List<TicketEntity>>();
        }
    }
}
