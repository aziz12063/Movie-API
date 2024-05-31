using ApiApplication.Database.Entities;
using ApiApplication.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using ApiApplication.Database.Repositories.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using ApiApplication.CustomExceptions;
using System.Reflection;
using ApiApplication.Database.Repositories;

namespace ApiApplication.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IAuditoriumsRepository _auditoriumRepository;
        private readonly IMovieService _movieService;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ShowtimeService(IAuditoriumsRepository auditoriumRepository,
                                IMovieService movieService,
                                IShowtimesRepository showtimesRepository,
                                IMapper mapper,
                                ILogger<ShowtimeService> logger)
        {

            _auditoriumRepository = auditoriumRepository;
            _movieService = movieService;
            _showtimesRepository = showtimesRepository;
            _mapper = mapper;
            _logger = logger;

        }


        public async Task<ShowtimeDto> CreateShowTime(ShowtimeDto showtimeDto, CancellationToken cancel)
        {
        
            if (showtimeDto == null)
            {
                _logger.LogError("The showtime is null");
                throw new ArgumentNullException(nameof(showtimeDto));
            }
    
            try
            {

                ShowtimeEntity showtimeEntity;
                try
                {
                    var audi = await _auditoriumRepository.GetByIdWithSeatsAndShowtimesAsync(showtimeDto.AuditoriumId, cancel);

                    showtimeEntity = _mapper.Map<ShowtimeEntity>(showtimeDto);
                    showtimeEntity.Auditorium = audi;
                    audi.Showtimes.Add(showtimeEntity);

                    if(showtimeEntity == null)
                    {
                        _logger.LogInformation("showtimeEntity not mapped");
                    }
                    else
                    {
                        // don
                        _logger.LogInformation("showtimeEntity  mapped");
                    }
                }
                
                catch (Exception ex) 
                {
                    _logger.LogError("canot map to showtimeEntity");
                    throw new Exception(ex.ToString());
                }

                ShowtimeDto showtimeDtoCreated = await _showtimesRepository.CreateShowtime(showtimeEntity, cancel);

                return showtimeDtoCreated;

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
        

        public async Task<ShowtimeDto> GetShowtimeByAuditoriumIdAndSessionDate(int auditoriumId, DateTime sessionDate, CancellationToken cancellationToken)
        {
            ShowtimeEntity showtimeEntity = await _showtimesRepository.GetByAuditoriumIdAndSessionDateAsync(auditoriumId, sessionDate, cancellationToken);
            if (showtimeEntity == null)
            {
                _logger.LogError("the showtime is null");
                return null;
            }

            ShowtimeDto showtimeDto = _mapper.Map<ShowtimeDto>(showtimeEntity);

            return showtimeDto;
        }

        public async Task<ShowtimeDto> GetShowtimeWithMovieById(int Id, CancellationToken cancellation)
        {
            ShowtimeEntity showtimeEntity = await _showtimesRepository.GetWithMoviesByIdAsync(Id, cancellation);
            return(_mapper.Map<ShowtimeDto>(showtimeEntity));
        }

        public async Task<bool> ShowtimeExistAsync(int auditoriumId, DateTime sessionDate)
        {
            return await _showtimesRepository.ShowtimeExistAsync(auditoriumId, sessionDate);
        }
    }
}
