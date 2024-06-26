using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class SeatService : ISeatService
    {
        private readonly ILogger<SeatService> _logger;

        
        public SeatService(ILogger<SeatService> logger)
        {
            _logger = logger;
        }

        public async Task<List<SeatEntity>> FindSeatsContiguous(IEnumerable<SeatEntity> availableSeats, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            return await Task.Run(() => {
                // group seats by row
                var groupedByRow = availableSeats.GroupBy(s => s.Row);

                foreach (var row in groupedByRow)
                {
                    var seatsInRow = row.OrderBy(s => s.SeatNumber).ToList();
                    for (int i = 0; i <= seatsInRow.Count - nbrOfSeatsToReserve; i++)
                    {
                        var seatsToReserve = seatsInRow.Skip(i).Take(nbrOfSeatsToReserve).ToList();
                        if (IsContiguous(seatsToReserve))
                        {
                            return seatsToReserve;
                        }
                    }
                }

                _logger.LogInformation("no seats contiguous found");

                return null;
            });
        }

        public bool IsContiguous(List<SeatEntity> seats)
        {

            for (int i = 0; i < seats.Count-1; i++)
            {
                if (seats[i].Row != seats[i+1].Row || seats[i].SeatNumber + 1 != seats[i + 1].SeatNumber)
                {
                    return false;
                }
            }
            return true;
        }

        // DRY in TicketService
        public async Task<List<SeatEntity>> UpdateSeatsState(List<SeatEntity> seats)
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

      
        public List<SeatDto> GrabSeatsAvailable(List<SeatDto> globalSeats, List<SeatDto> reservedSeats) 
        {
            //List<SeatDto> availablSeatsDto;
            foreach(var seat in globalSeats)
            {
                foreach(var s in reservedSeats)
                {
                    if(seat.Row == s.Row && seat.SeatNumber == s.SeatNumber)
                    {
                        globalSeats.Remove(seat);
                    }
                }
            }
            //availablSeatsDto = globalSeats.Except(reservedSeats, new SeatDtoEqualityComparer()).ToList();
            return globalSeats;
        }
   
    }

        
}
