using ApiApplication.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Services.Interfaces
{
    public interface ISeatService
    {
        //Task<List<SeatDto>> GettSeats(int auditoriumId);
        Task<List<SeatDto>> FindSeatsContiguous(int auditoriumId, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto);
        Task<List<SeatDto>> UpdateSeatsState(List<SeatDto> seats);
    }
}
