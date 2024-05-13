using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ISeatService _seatService;
        //private readonly IShowtimeService _showtimeService;
        private readonly CinemaContext _dbContext;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationService> _logger;
        List<SeatDto>  listOfSeatsreserved = new List<SeatDto>();


        public ReservationService(ISeatService seatService,
                                  IShowtimesRepository showtimesRepository,
                                  CinemaContext dbContext,
                                  IMapper mapper,
                                   ILogger<ReservationService> logger)
        {
            _seatService = seatService;
            //_showtimeService = showtimeService;
            _showtimesRepository = showtimesRepository;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        private void UpdateIsReserved()
        {
            
        }

        
        private async Task<TicketDto> CreateTicketDtoToReserveAsync(int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            Guid guid = Guid.NewGuid();

            // I grab the ShowTime:
            var showtime = await _showtimesRepository.GetWithSeatsByIdAsync(showtimeId, cancel);
            if(showtime == null)
            {
                _logger.LogError("no showtime found");

                return null;
            }

            ShowtimeDto showtimeDto = _mapper.Map<ShowtimeDto>(showtime);

            // get the auditoriumId:
            int auditoriumId = showtimeDto.AuditoriumId;

            // Find Seats Contiguous:
            var listSeatsToReserve = await FindSeatsContiguous(auditoriumId, nbrOfSeatsToReserve, showtimeDto);

            TicketDto reservationDto = new()
            {
                Id = guid,
                ShowtimeId = showtimeId,
                Seats = listSeatsToReserve,
                CreatedTime = DateTime.Now,
                Paid = false,
                Showtime = showtimeDto

            };
            return  reservationDto;

           
        }
        
        public async Task CreateTicketAsync (int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            var reservationDto = await CreateTicketDtoToReserveAsync(showtimeId, nbrOfSeatsToReserve, cancel);

            // Start the delay task with the provided CancellationToken
            var delayTask = Task.Delay(TimeSpan.FromMinutes(10), cancel);

            // ContinueWith the cancellation logic
            await delayTask.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    _logger.LogInformation("Delay canceled before timeout");
                }
                else
                {
                    HandleCancellation(reservationDto);
                }
            });


            return;
        }

        private void HandleCancellation(TicketDto reservation)
        {
            // Check if seats are paid after 10 minutes
            if (!reservation.Paid)
            {
                // If seats are not paid, cancel the reservation
                // Perform cancellation logic here
                // For example, set a reservation status flag to canceled
                _logger.LogInformation("Reservation {ReservationId} canceled because seats were not paid.", reservation.Id);
            }
        }

       


        // recap this code, DRY
        private async Task<List<SeatDto>> FindSeatsContiguous(int auditoriumId, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto)
        {
            return await Task.Run(() =>
            {
                List<SeatDto> listSeatReserved = new List<SeatDto>();

                int nbrOfSeatsPerRow;
                int nbrOfSeatsAvailable;
                int rowNbr;
                int seatNbr;
                int nbrOfSeatsContiguous = 0;
                int index = 0;
                int nbrOfRow;

                // get the list of seats :
                var seats = showtimeDto.Seats.ToList();

                var seat = seats.FirstOrDefault(s => s.IsReserved == false);
                
                if(seat == null)
                {
                    _logger.LogError("no more seats available");
                    return null;
                }

                listOfSeatsreserved.Add(seat);

                if (nbrOfSeatsToReserve == 1)
                {
                    seat.IsReserved = true;
                    return listOfSeatsreserved;
                }

                switch (auditoriumId)
                {
                    case 1:
                        nbrOfSeatsPerRow = 22;
                        nbrOfRow = 28;
                        break;
                    case 2:
                        nbrOfSeatsPerRow = 18;
                        nbrOfRow = 21;
                        break;
                    default:
                        nbrOfSeatsPerRow = 21;
                        nbrOfRow = 15;
                        break;
                }

                if(nbrOfSeatsToReserve > nbrOfSeatsPerRow)
                {
                    _logger.LogInformation("there are no enough seats contiguous");
                    return null;
                }


                rowNbr = seat.Row;
                seatNbr = seat.SeatNumber;

                nbrOfSeatsAvailable = nbrOfSeatsPerRow - seatNbr + 1;

                // grab the index of seat:
                index = seats.IndexOf(seat);
 

                for(int r = rowNbr; r <= nbrOfRow; r++ )
                {
                    
                    for(int s = seatNbr+1; s <= nbrOfSeatsPerRow; s++)
                    {
                        index++;

                        if (seats[index].IsReserved == false)
                        {
                            nbrOfSeatsContiguous++;
                            listOfSeatsreserved.Add(seats[index]);

                            if(nbrOfSeatsContiguous == nbrOfSeatsToReserve )
                            {
                                // i should make them reserved, for that use for loop with index -- and nbrseatsToReserve
                                return listOfSeatsreserved;
                            }
                        }
                        else
                        {
                            nbrOfSeatsContiguous = 0;
                            listOfSeatsreserved.Clear( );
                        }
                        
                    }
                    listOfSeatsreserved.Clear();
                    nbrOfSeatsContiguous = 0;
                    seatNbr = 0;
                    

                }

                return null;
                


                // check 10 min validation reservation
            });
        }

       
    }
}
