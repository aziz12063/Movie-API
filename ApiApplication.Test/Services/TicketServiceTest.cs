using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database;
using ApiApplication.Models;
using ApiApplication.Services;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using ApiApplication.Database.Entities;
using Xunit;

namespace ApiApplication.Test.Services
{
    public class TicketServiceTest
    {
       
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ISeatService> _mockSeatService;
        private readonly Mock<IShowtimesRepository> _mockShowtimesRepository;
        private readonly Mock<ILogger<TicketService>> _mockLogger;
        private readonly Mock<ITicketsRepository> _mockTicketsRepository;
        CancellationToken _cancel;

        private readonly Mock<IMemoryCache> _mockCache;


        //private Dictionary<Guid, TicketDto> _tickets = new();
        private Dictionary<Guid, Timer> _timers = new();

        private const string TicketsCachKey = "Tickets";

        private TicketService _ticketService;

        public TicketServiceTest()
        {
           
            _mockMapper = new Mock<IMapper>();
            _mockSeatService = new Mock<ISeatService>();
            _mockShowtimesRepository = new Mock<IShowtimesRepository>();
            _mockLogger = new Mock<ILogger<TicketService>>();
            _mockTicketsRepository = new Mock<ITicketsRepository>();
            _mockCache = new Mock<IMemoryCache>();

            _ticketService = new TicketService(_mockMapper.Object,
                                               _mockShowtimesRepository.Object,
                                               _mockSeatService.Object,
                                               _mockLogger.Object,
                                               _mockTicketsRepository.Object,
                                               _mockCache.Object);

            _cancel = new CancellationToken();

        }


        [Fact]
        public async void CreateTicketWithDelayAsync_GetWithAuditAndTicketsAndSeatsReturnNull()
        {
            TicketDto ticketDto = new();
            int nbrOfSeatsToReserve = 2;
            ShowtimeEntity? showtimeEntity = null;
           
            _mockShowtimesRepository.Setup(x => x.GetWithAuditAndTicketsAndSeats(It.IsAny<int>(), _cancel)).ReturnsAsync(showtimeEntity);

            var result = await _ticketService.CreateTicketWithDelayAsync(ticketDto, nbrOfSeatsToReserve, _cancel);

            Assert.Null(result);
        }

        [Fact]
        public async void CreateTicketWithDelayAsync_FindSeatsContiguousReturnNull()
        {
            TicketDto ticketDto = new TicketDto {TicketId = new Guid() };
            int nbrOfSeatsToReserve = 2;

            ShowtimeEntity showtimeEntity = new ShowtimeEntity {showtimeId = 1, AuditoriumId = 1, Auditorium = new(), Movie = new(), SessionDate = DateTime.Now };

            List<SeatEntity>? seats = null;

            _mockShowtimesRepository.Setup(x => x.GetWithAuditAndTicketsAndSeats(It.IsAny<int>(), _cancel)).ReturnsAsync(showtimeEntity);
            _mockSeatService.Setup(x => x.FindSeatsContiguous(It.IsAny<List<SeatEntity>>(), It.IsAny<int>(), _cancel)).ReturnsAsync(seats);


            var result = await _ticketService.CreateTicketWithDelayAsync(ticketDto, nbrOfSeatsToReserve, _cancel);

            Assert.Null(result);
        }





        //[Fact]
        //public void TheCollectionOfTickerAreGenerated()
        //{
        //    //int nbrOfTicket = 3;
        //    //List<Guid> guid = new List<Guid>();
        //    //guid.Add(new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA"));
        //    //guid.Add(new Guid("6B30FC40-CA47-1067-B31D-00DD010662DB"));
        //    //guid.Add(new Guid("6B30FC40-CA47-1067-B31D-00DD010662DC"));


        //    //TicketService ticketService = new TicketService();

        //    //List<TicketDto> tickets = ticketService.GenerateTicketCollection(guid);

        //    //TicketDto ticketTest = new TicketDto
        //    //{
        //    //    Id = Guid.NewGuid(),
        //    //    ShowtimeId = 1,
        //    //    CreatedTime = DateTime.Now
        //    //};

        //    //// verify the list of Ticket
        //    //Assert.NotNull(tickets);

        //    //// the nbrOfTicket will be the nbr of seat in an auditorium
        //    //Assert.Equal(nbrOfTicket, tickets.Count);

        //    //// verify each Ticket
        //    //TicketDto ticket = tickets[0];
        //    //TicketDto ticket1 = tickets[1];
        //    //TicketDto ticket2 = tickets[2];

        //    //Assert.NotNull(ticket);
        //    //Assert.NotNull(ticket1);
        //    //Assert.NotNull(ticket2);


        //    //// verify the members of Ticket:
        //    //Assert.Equal(guid[0], ticket.Id);
        //    //Assert.Equal(guid[1], ticket1.Id);

        //    //// verify the showtime

        //    //Assert.Equal(1, ticket1.ShowtimeId);
        //    //Assert.Equal(2, ticket2.ShowtimeId);


        //    //// modify the time to now
        //    //Assert.Equal(ticket1.CreatedTime, new DateTime(2024, 03, 05, 14, 00, 00));
        //    //Assert.Equal(ticket2.CreatedTime, new DateTime(2024, 03, 05, 16, 00, 00));




        //}
    }
}
