using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
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

        //public async  Task<List<TicketDto>> GetTicketCollection(int auditoriumId, int showtimeId)
        //{
        //    if (!VerifyIfWeStillHaveTicket())
        //    {
        //        throw new Exception($"all the tickets are seals");
        //    }
        //    // i should verify the auditoriumId

        //    // 

        //    var ticketCollection = await GenerateTicketCollection(auditoriumId, showtimeId);

           

        //    return ticketCollection;
        //}

        //private async Task<List<TicketDto>> GenerateTicketCollection(int auditoriumId, int showtimeId)
        //{
        //    var ticketCollection = new List<TicketDto>();
        //    int nbrOfTicketToGenerate = 0;
        //    switch (auditoriumId)
        //    {
        //        case 1:
        //            nbrOfTicketToGenerate = 616;
        //            break;
        //        case 2:
        //            nbrOfTicketToGenerate = 378;
        //            break;
        //        case 3:
        //            nbrOfTicketToGenerate = 315;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(auditoriumId));
        //    }

        //    var seat = await _seatService.GettSeats(auditoriumId);

        //    for (int i = 0; i < nbrOfTicketToGenerate; i++)
        //    {
        //        ticketCollection.Add(new TicketDto
        //        {
        //            Id = new Guid(),
        //            Seats = (ICollection<SeatDto>)seat, // must be SeatEntity or SeatDto ???
        //            ShowtimeId = showtimeId,
                    
        //        });
        //    }

        //    return ticketCollection;
        //}

        
    }
}
