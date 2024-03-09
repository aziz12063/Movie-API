using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;

namespace ApiApplication.Profiles
{
    public class AuditoriumProfile : Profile
    {
        public AuditoriumProfile()
        {
            CreateMap<AuditoriumEntity, AuditoriumDto>();
            CreateMap<AuditoriumDto, AuditoriumEntity>();
        }
    }
}
