using ApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication
{
    public interface IReservationService
    {
        Task<ReservationDto> ReserveSeat(int showtimeId, int nbrOfSeatsToReserve);
        Task<List<SeatDto>> CheckSeatsContiguous(int auditoriuomId, int nbrOfSeatsToReserve, List<SeatDto> seats, int index, int row);
    }
}
