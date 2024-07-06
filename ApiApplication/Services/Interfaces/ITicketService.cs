using ApiApplication.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services.Interfaces
{
    public interface ITicketService
    {
      
        Task<TicketDto> CreateTicketWithDelayAsync(TicketDto ticketDto, int nbrOfSeatsToReserve, CancellationToken cancel);
        Task<bool> ConfirmPayementAsync(Guid id, CancellationToken cancellation);
    }
}
