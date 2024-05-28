using ApiApplication.Database;
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
        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<SeatService> _logger;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IAuditoriumsRepository _auditoriumsRepository;

        //public List<SeatDto> seats = new List<SeatDto>();
        public SeatService(CinemaContext dbContext, IShowtimesRepository showtimesRepository, IMapper mapper, IAuditoriumService auditoriumService, ILogger<SeatService> logger, IAuditoriumsRepository auditoriumsRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _auditoriumService = auditoriumService;
            _showtimesRepository = showtimesRepository;
            _auditoriumsRepository = auditoriumsRepository;
        }

        public async Task<List<SeatDto>> FindSeatsContiguous(IEnumerable<SeatDto> availableSeatsDto, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto, CancellationToken cancel)
        {
            return await Task.Run(() => {
                // group seats by row
                var groupedByRow = availableSeatsDto.GroupBy(s => s.Row);

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

        private bool IsContiguous(List<SeatDto> seats)
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
