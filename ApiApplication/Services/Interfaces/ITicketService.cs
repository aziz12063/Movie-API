using ApiApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication
{
    public interface ITicketService
    {
        //bool VerifyIfWeStillHaveTicket();
        //Task<List<TicketDto>> GetTicketCollection(int auditoriumId, int showtimeId);
        Task<TicketDto> CreateTicketWithDelayAsync(IEnumerable<SeatDto> availableSeatsDto, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto, CancellationToken cancel);
        Task ConfirmPayementAsync(Guid id, CancellationToken cancellation);
    }
}
