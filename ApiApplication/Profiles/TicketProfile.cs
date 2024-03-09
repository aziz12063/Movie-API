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
            CreateMap<TicketEntity, TicketDto>(); 
            //CreateMap<List<TicketEntity>, List<TicketDto>>();

            CreateMap<TicketDto, TicketEntity>().ForMember(dest => dest.Showtime, opt => opt.Ignore())
                                                .ForMember(dest => dest.Seats, opt => opt.Ignore());
            //CreateMap<List<TicketDto>, List<TicketEntity>>();
        }
    }
}
