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

            // review this piece of code
            MovieDto movie;
            try
            {
                 movie = await _movieService.GetMovieById(showtimeDto.Movie.movieId);
            }
            catch (Exception ex)
            {
                throw new Exception("error occur while fetching MovieById", ex);
            }


            if (movie == null)
            {
                _logger.LogWarning("The Movie with Id: {showtimeDto.Movie.Id} doesn't exist.", showtimeDto.Movie.movieId);
                throw new InvalidInPutException("the Movie is null");
            }
            
            AuditoriumEntity auditoriumEntity;
            try
            {
                // i get the auditorium using auditoriumId including showtimes
                auditoriumEntity = await _auditoriumRepository.GetByIdIncludShowtimeAsync(showtimeDto.AuditoriumId, cancel);

            }
            catch (Exception)
            {
                throw;
            }
            
            
            if (auditoriumEntity == null)
            {
                _logger.LogWarning("Auditorium with Id: {showtimeDto.AuditoriumId} doesn't exist.", showtimeDto.AuditoriumId);
                throw new InvalidInPutException("Invalid auditoriumId from ShowtimeService class, " +  base.ToString()); // or i can : ObjectNotFoundException
            }

            if ((bool)auditoriumEntity.Showtimes?.Any(showtime => showtime.SessionDate == showtimeDto.SessionDate))
            {
                _logger.LogWarning("Showtime in the Auditorium: {showtimeDto.AuditoriumId} in this date: {showtimeDto.SessionDate} already exist"
                                    , showtimeDto.AuditoriumId, showtimeDto.SessionDate);
                return null;
            }
            
    
            try
            {
                showtimeDto.Movie = movie;
              
                // log all the properties of the movie fetched
                // i will delete it later
                var properties = showtimeDto.Movie.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var value = property.GetValue(showtimeDto.Movie);
                    _logger.LogInformation($"{property.Name}: {value}");
                }

                ShowtimeEntity showtimeEntity = _mapper.Map<ShowtimeEntity>(showtimeDto);

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
    }
}
