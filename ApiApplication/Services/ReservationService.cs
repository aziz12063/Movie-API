using ApiApplication.Database;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ISeatService _seatService;
        //private readonly IShowtimeService _showtimeService;
        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        List<SeatDto>  listOfSeatsreserved = new List<SeatDto>();


        public ReservationService(ISeatService seatService, CinemaContext dbContext, IMapper mapper)
        {
            _seatService = seatService;
            //_showtimeService = showtimeService;
            _dbContext = dbContext;
            _mapper = mapper;
            
        }

        private void UpdateIsReserved()
        {
            
        }

        /*
        public async Task<ReservationDto> ReserveSeat(int showtimeId, int nbrOfSeatsToReserve)
        {
            
            // i get the Sowtime object
            var showtime = DataStore.ShowtimesDto.Find(c => c.Id == showtimeId);
            if (showtime == null)
            {
                throw new ArgumentException("showtime not found");
            }

           

            // grab the AuditoriumId
            int auditoriuomId = showtime.AuditoriumId;

            // fetch the Movie in this showtimeId:
            var movie = showtime.Movie;

            // get the list of SeatsDto in this auditorium:
            var seats = await _seatService.GettSeats(auditoriuomId);

            // generate the Guid:
            Guid reservationGuid = Guid.NewGuid();
            ReservationDto reservationDto = new();
            reservationDto.Id = reservationGuid;
            reservationDto.AuditoriumId = auditoriuomId;
            reservationDto.MovieName = movie.Title;

            // grab the first seat non-reserved
            var seatToReserve = seats.FirstOrDefault(c => c.IsReserved == false);
            int index = seats.IndexOf(seatToReserve);

            if (nbrOfSeatsToReserve == 1)
            {
                seatToReserve.IsReserved = true;
                reservationDto.listOfSeatsreserved.Add(seatToReserve);
                
                
                
            }
            else
            {
                listOfSeatsreserved = await CheckSeatsContiguous(auditoriuomId, nbrOfSeatsToReserve, seats, index, seatToReserve.Row);

                foreach (var seat in listOfSeatsreserved)
                {
                    seat.IsReserved = true;
                    reservationDto.listOfSeatsreserved.Add(seat);
                }
            }
            reservationDto.ReservationTime = DateTime.Now;
            reservationDto.IsExpired = false;


            return reservationDto;
        }
        */

        // recap this code, DRY
        public async Task<List<SeatDto>> CheckSeatsContiguous(int auditoriuomId, int nbrOfSeatsToReserve, List<SeatDto> seats, int index, int row)
        {
            return await Task.Run(() =>
            {
                int nbrOfSeatsPerRow;
                int nbrOfSeatsAvailable;
                int nbrOfRowThatHasThisIndex;
                int nbrOfSeatsContiguous = 0;
                int indexInTempList = 0;
                int maxRow;
                int initialRow = row;


                List<SeatDto> listSeatReserved = new List<SeatDto>();

                switch (auditoriuomId)
                {
                    case 1:
                        nbrOfSeatsPerRow = 22;
                        maxRow = 28;
                        break;
                    case 2:
                        nbrOfSeatsPerRow = 18;
                        maxRow = 21;
                        break;
                    default:
                        nbrOfSeatsPerRow = 21;
                        maxRow = 15;
                        break;
                }

                nbrOfSeatsAvailable = nbrOfSeatsPerRow - ((index + 1) % nbrOfSeatsPerRow);

                // check if the row that not contain other contiguous seats.
                // are there some other contiguous.

                // fetch the nbr of row that contain the seat with index:
                nbrOfRowThatHasThisIndex = (index + 1) / nbrOfSeatsPerRow;

                var tempList = seats.Skip(index).Take(nbrOfSeatsAvailable).ToList();

                for (int i = 0; i < nbrOfSeatsAvailable; i++)
                {

                    if (tempList[i].IsReserved == false)
                    {
                        nbrOfSeatsContiguous++;
                        if (nbrOfSeatsContiguous == nbrOfSeatsToReserve)
                        {
                            indexInTempList = i - nbrOfSeatsToReserve + 1;
                            for (int j = 0; j < nbrOfSeatsToReserve; j++)
                            {
                                listSeatReserved.Add(tempList[indexInTempList]);
                                indexInTempList++;
                            }
                            return listSeatReserved; ;

                        }
                    }
                    nbrOfSeatsToReserve = 0;
                }

                do
                {
                    tempList.Clear();

                    tempList = seats.Skip(row * nbrOfSeatsPerRow).Take(nbrOfSeatsPerRow).ToList();

                    for (int i = 0; i < nbrOfSeatsPerRow; i++)
                    {

                        if (tempList[i].IsReserved == false)
                        {
                            nbrOfSeatsContiguous++;
                            if (nbrOfSeatsContiguous == nbrOfSeatsToReserve)
                            {
                                indexInTempList = i - nbrOfSeatsToReserve + 1;
                                for (int j = 0; j < nbrOfSeatsToReserve; j++)
                                {
                                    listSeatReserved.Add(tempList[indexInTempList]);
                                    indexInTempList++;
                                }
                                return listSeatReserved;

                            }
                        }
                        nbrOfSeatsToReserve = 0;
                    }

                    row++;

                    if (row == maxRow + 1)
                    {
                        throw new ArgumentOutOfRangeException("there's no seats contiguous for your reservation");

                    }

                } while (true);


                // check 10 min validation reservation
            });
        }

        public Task<ReservationDto> ReserveSeat(int showtimeId, int nbrOfSeatsToReserve)
        {
            throw new NotImplementedException();
        }
    }
}
