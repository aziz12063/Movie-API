using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class TicketService : ITicketService
    {

        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISeatService _seatService;

        public TicketService(CinemaContext dbContext, IMapper mapper, ISeatService seatService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _seatService = seatService;
        }
        

        // i add here a collection of Seats of a Showtime
        public bool VerifyIfWeStillHaveTicket()
        {
            // implement the logic and modify the return
            return true;
        }

        public async Task ConfirmPayementAsync(TicketDto tocketDto, CancellationToken cancellation)
        {
            // check if the reservation is still alive
            // call ConfirmPayementAsync from TicketRepo
            return;
        }
        
    }
}
