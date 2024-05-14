using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class SeatService : ISeatService
    {
        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<SeatService> _logger;
        //public List<SeatDto> seats = new List<SeatDto>();
        public SeatService(CinemaContext dbContext, IMapper mapper, ILogger<SeatService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<List<SeatDto>> FindSeatsContiguous(int auditoriumId, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto)
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

                if (seat == null)
                {
                    _logger.LogError("no more seats available");
                    return null;
                }

                listSeatReserved.Add(seat);

                if (nbrOfSeatsToReserve == 1)
                {
                    seat.IsReserved = true;
                    return listSeatReserved;
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

                if (nbrOfSeatsToReserve > nbrOfSeatsPerRow)
                {
                    _logger.LogInformation("there are no enough contiguous seats available");
                    return null;
                }


                rowNbr = seat.Row;
                seatNbr = seat.SeatNumber;

                nbrOfSeatsAvailable = nbrOfSeatsPerRow - seatNbr + 1;

                // grab the index of seat:
                index = seats.IndexOf(seat);


                for (int r = rowNbr; r <= nbrOfRow; r++)
                {

                    for (int s = seatNbr + 1; s <= nbrOfSeatsPerRow; s++)
                    {
                        index++;

                        if (seats[index].IsReserved == false)
                        {
                            nbrOfSeatsContiguous++;
                            listSeatReserved.Add(seats[index]);

                            if (nbrOfSeatsContiguous == nbrOfSeatsToReserve)
                            {
                                // i should make them reserved, for that use for loop with index -- and nbrseatsToReserve
                                return listSeatReserved;
                            }
                        }
                        else
                        {
                            nbrOfSeatsContiguous = 0;
                            listSeatReserved.Clear();
                        }

                    }
                    listSeatReserved.Clear();
                    nbrOfSeatsContiguous = 0;
                    seatNbr = 0;

                }
                _logger.LogInformation("No contiguous seats found");
                return null;

            });


        }

        public async Task<List<SeatDto>> UpdateSeatsState(List<SeatDto> seats)
        {
            return await Task.Run(() =>
            {
                foreach (var seat in seats)
                {
                    seat.IsReserved = !seat.IsReserved;
                }
                return seats;
            });
        }
    }
}
