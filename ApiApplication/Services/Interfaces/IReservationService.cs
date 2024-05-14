using ApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication
{
    public interface IReservationService
    {
        Task<ReservationDto> ReserveSeatAsync(int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel);
        
    }
}
