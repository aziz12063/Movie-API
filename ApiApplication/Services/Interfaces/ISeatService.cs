using ApiApplication.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services.Interfaces
{
    public interface ISeatService
    {
        //Task<List<SeatDto>> GettSeats(int auditoriumId);
        Task<List<SeatDto>> FindSeatsContiguous(IEnumerable<SeatDto> availableSeatsDto, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto, CancellationToken cancel);
        Task<List<SeatDto>> UpdateSeatsState(List<SeatDto> seats);
        List<SeatDto> GrabSeatsAvailable(List<SeatDto> globalSeats, List<SeatDto> reservedSeats);

        List<SeatDto> GenerateSeats(int auditoriumId);
    }
}
