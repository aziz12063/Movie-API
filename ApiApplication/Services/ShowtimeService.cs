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
using System.Collections.Generic;

namespace ApiApplication.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IAuditoriumsRepository _auditoriumRepository;
        private readonly IMovieService _movieService;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;


        public int Id { get; private set; }
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


        public async Task<bool> CreateShowTime(ShowtimeDto showtimeDto, CancellationToken cancel)
        {
            if (showtimeDto == null)
            {
                _logger.LogError("The showtime is null");
                throw new ArgumentNullException(nameof(showtimeDto));
            }

            MovieDto movie = new MovieDto();
            try
            {
                movie = await _movieService.GetMovieById(showtimeDto.Movie.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("error occur while fetching MovieByIg", ex);
            }


            if (movie == null)
            {
                _logger.LogWarning("The Movie with Id: {showtimeDto.Movie.Id} doesn't exist.", showtimeDto.Movie.Id);
                throw new InvalidInPutException("the Movie is null");
            }

            
            AuditoriumEntity auditoriumEntity = new AuditoriumEntity();
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
                throw new InvalidInPutException(); // or i can : ObjectNotFoundException
            }

            if ((bool)auditoriumEntity.Showtimes?.Any(showtime => showtime.SessionDate == showtimeDto.SessionDate))
            {
                _logger.LogWarning("Showtime in the Auditorium: {showtimeDto.AuditoriumId} in this date: {showtimeDto.SessionDate} already exist"
                                    , showtimeDto.AuditoriumId, showtimeDto.SessionDate);
                return false;
            }
            

            
            int nbrOfMemberSaved = 0;
            try
            {
                showtimeDto.Movie = movie;


                showtimeDto.Seats = GenerateSeats(showtimeDto.AuditoriumId);

                ShowtimeEntity showtimeEntity = _mapper.Map<ShowtimeEntity>(showtimeDto);

                nbrOfMemberSaved = await _showtimesRepository.CreateShowtime(showtimeEntity, cancel);

                //if (nbrOfMemberSaved == showtimeEntity.GetType().GetProperties().Length)
                //{
                //    return true;
                //}

                return true;

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        private static List<SeatDto> GenerateSeats(int auditoriumId)
        {
            short rows;
            short seatsPerRow;

            switch (auditoriumId)
            {
                case 1:
                    seatsPerRow = 22;
                    rows = 28;
                    break;
                case 2:
                    seatsPerRow = 18;
                    rows = 21;
                    break;
                default:
                    seatsPerRow = 21;
                    rows = 15;
                    break;
            }

            var seats = new List<SeatDto>();
            for (short r = 1; r <= rows; r++)
                for (short s = 1; s <= seatsPerRow; s++)
                    seats.Add(new SeatDto { AuditoriumId = auditoriumId, Row = r, SeatNumber = s });

            return seats;
        }
    }
}
