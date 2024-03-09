using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Collections.Generic;
using System.Linq;

namespace ApiApplication.Profiles
{
    public class ShowtimeProfile : Profile
    {
        public ShowtimeProfile() 
        {
            // we create a map from ShitimeEntity to ShowtimeDto
            CreateMap<ShowtimeEntity, ShowtimeDto>();
            //CreateMap<List<ShowtimeEntity>, List<ShowtimeDto>>();



            // we create a map from ShowtimeDto to ShitimeEntity
            CreateMap<ShowtimeDto, ShowtimeEntity>().ForMember(dest => dest.Tickets, opt => opt.Ignore());
                                                    
            //CreateMap<List<ShowtimeDto>, List<ShowtimeEntity>>();

            
        }
    }
}
