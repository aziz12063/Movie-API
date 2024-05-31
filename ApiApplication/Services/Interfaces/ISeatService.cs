using ApiApplication.Database.Entities;
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
        Task<List<SeatEntity>> FindSeatsContiguous(IEnumerable<SeatEntity> availableSeatsDto, int nbrOfSeatsToReserve, CancellationToken cancel);
        Task<List<SeatEntity>> UpdateSeatsState(List<SeatEntity> seats);
        List<SeatDto> GrabSeatsAvailable(List<SeatDto> globalSeats, List<SeatDto> reservedSeats);

        List<SeatDto> GenerateSeats(int auditoriumId);
    }
}
