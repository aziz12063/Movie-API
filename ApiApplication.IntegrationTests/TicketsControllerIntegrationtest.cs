using ApiApplication.Controllers;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database.Repositories;
using ApiApplication.Database;
using ApiApplication.IntegrationTests.FixtureClassesFirIntegration;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using ApiApplication.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace ApiApplication.IntegrationTests
{
    public class TicketsControllerIntegrationtest : IClassFixture<DbFixtureIntegration>
    {
        private readonly ITicketService _ticketService;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly ISeatService _seatService;
        private readonly ITicketsRepository _ticketsRepository;


        private readonly ILogger<TicketsController> _loggerTicketsController;
        private readonly ILogger<ShowtimesRepository> _loggerShowtimeRepo;
        private readonly ILogger<SeatService> _loggerSeatService;
        private readonly ILogger<TicketService> _loggerTicketService;
        private readonly ILogger<TicketsRepository> _loggerTicketsRepository;
        

        private readonly IMemoryCache _cache;

        private DbFixtureIntegration _dbFixtureShared;
        private CinemaContext _dbContext;
        private readonly TicketsController _ticketsController;
        private readonly IMapper _mapper;


        public TicketsControllerIntegrationtest(DbFixtureIntegration dbFixtureShared)
        {
            _dbFixtureShared = dbFixtureShared;
            _dbContext = _dbFixtureShared.context;

            _mapper = _dbFixtureShared._mapper;
            var loggerFactory = _dbFixtureShared._loggerFactory;

            _loggerSeatService = loggerFactory.CreateLogger<SeatService>();
            _loggerTicketService = loggerFactory.CreateLogger<TicketService>();
            _loggerTicketsRepository = loggerFactory.CreateLogger<TicketsRepository>();
            _loggerTicketsController = loggerFactory.CreateLogger<TicketsController>();
            _loggerShowtimeRepo = loggerFactory.CreateLogger<ShowtimesRepository>();    

            _cache = new MemoryCache(new MemoryCacheOptions());

            _showtimesRepository = new ShowtimesRepository(_dbContext,
                                                           _mapper);

            _seatService = new SeatService(_loggerSeatService);

            _ticketsRepository = new TicketsRepository(_dbContext,
                                                       _loggerTicketsRepository );

            _ticketService = new TicketService(_mapper,
                                               _showtimesRepository,
                                               _seatService,
                                               _loggerTicketService,
                                               _ticketsRepository,
                                               _cache);



            _auditoriumsRepository = new AuditoriumsRepository(_dbContext);
            

            _ticketsController = new TicketsController(_ticketService,
                                                       _loggerTicketsController
                                                       //_showtimesRepository

                                                     //_mapper
                                                       );



        }

        [Fact]
        public async void CreateTicket_Success_ReturnTicketDto()
        {
            int showtimeId = 10;
            int nbrOfSeats = 2;
            
            CancellationToken cancellationToken = new CancellationToken();

            var result = await _ticketsController.CreateTicket(showtimeId, nbrOfSeats, cancellationToken);

            Assert.NotNull(result);
            var actionResult = Assert.IsType<ActionResult<TicketDto>>(result);

            var createdAtRouteResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var resultDto = Assert.IsType<TicketDto>(createdAtRouteResult.Value);
           
            Assert.NotNull(resultDto);

            Assert.Equal(showtimeId, resultDto.ShowtimeId);
        
        }


    }


}

