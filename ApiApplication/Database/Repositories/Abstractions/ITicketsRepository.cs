﻿using ApiApplication.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Database.Repositories.Abstractions
{
    public interface ITicketsRepository
    {
        Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel);
        Task<TicketEntity> CreateAsync(ShowtimeEntity showtime, IEnumerable<SeatEntity> selectedSeats, CancellationToken cancel);
        Task<TicketEntity> GetByIdAsync(Guid id, CancellationToken cancel);
        Task<IEnumerable<TicketEntity>> GetByShowtimeIdAsync(int showtimeId, CancellationToken cancel);
        Task<TicketEntity> CreateAsync(TicketEntity ticketEntity, CancellationToken cancel);
    }
}