﻿using ApiApplication.Database.Entities;
using ApiApplication.Models;
using ApiApplication.ProvidedApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;
using Serilog.Parsing;

namespace ApiApplication.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<MovieEntity, MovieDto>();
            CreateMap<MovieDto, MovieEntity>().ForMember(dest => dest.Showtimes, opt => opt.Ignore())
                                              .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<MoviesApiEntity, MovieDto>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                                                  .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                                                  .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => src.Crew))
                                                  .ForMember(dest => dest.ImdbId, opt => opt.MapFrom(src => src.ImDbRating))
                                                  //.ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => DateTime.Parse(src.year)))
                                                  .ForMember(dest => dest.ReleaseDate, opt => opt.Ignore());
        }
    }
}
