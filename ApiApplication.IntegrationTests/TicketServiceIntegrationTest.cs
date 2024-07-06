using ApiApplication.CustomExceptions;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database.Repositories;
using ApiApplication.Database;
using ApiApplication.IntegrationTests.FixtureClassesFirIntegration;
using ApiApplication.Models;
using ApiApplication.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ApiApplication.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Serilog.Core;

namespace ApiApplication.IntegrationTests
{
    //[Collection("DB collection")]
    public class TicketServiceIntegrationTest : IClassFixture<DbFixtureIntegration>
    {
        private readonly IMapper _mapper;
        private readonly ISeatService _seatService;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly ITicketsRepository _ticketRepository;

        private readonly ILogger<TicketService> _loggerTicketService;
        private readonly ILogger<SeatService> _loggerSeatService;
        private readonly ILogger<TicketsRepository> _loggerTicketRepo;

        private readonly TicketService _ticketService;

        private readonly IMemoryCache _cache;

        private DbFixtureIntegration _dbFixtureShared;
        private CinemaContext _dbContext;

        private Dictionary<Guid, Timer> _timers = new();
        private const string TicketsCachKey = "Tickets";

        public TicketServiceIntegrationTest(DbFixtureIntegration dbFixtureShared)
        {
            _dbFixtureShared = dbFixtureShared;
            _dbContext = _dbFixtureShared.context;

            var config = new MapperConfiguration(cfg =>
            {
                // Automatically load profiles from the assembly containing the profiles
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });
            _mapper = config.CreateMapper();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });

            _loggerTicketService = loggerFactory.CreateLogger<TicketService>();
           
            _loggerSeatService = loggerFactory.CreateLogger<SeatService>();
            _loggerTicketRepo = loggerFactory.CreateLogger<TicketsRepository>();

            _cache = new MemoryCache(new MemoryCacheOptions());

            _showtimesRepository = new ShowtimesRepository(_dbContext, _mapper);
            _seatService = new SeatService(_loggerSeatService);
            _ticketRepository = new TicketsRepository(_dbContext, _loggerTicketRepo);

            _ticketService = new TicketService(_mapper,
                                               _showtimesRepository,
                                               _seatService,
                                               _loggerTicketService,
                                               _ticketRepository,
                                               _cache);


        }

        [Fact]
        public async void CreateTicketWithDelayAsync_Success_ReturnTicketDto()
        {
            var showtimeId = 10;
            var ticketDto = new TicketDto { CreatedTime = new DateTime(2024, 7, 2), ShowtimeId = showtimeId };
            Guid guid = Guid.NewGuid();
            var nbrOfSeatsToreserv = 2;
            
           

            var showtimeEntity = await _showtimesRepository.GetWithAuditAndTicketsAndSeats(showtimeId, _dbFixtureShared.cancellationToken);
            var availableSeatsEntity = showtimeEntity.Auditorium.Seats.Where(s => s.IsReserved == false).ToList();
            var seatsToReserve = await _seatService.FindSeatsContiguous(availableSeatsEntity, nbrOfSeatsToreserv, _dbFixtureShared.cancellationToken);
            Assert.NotEmpty(seatsToReserve);

            var ticketEntity = new TicketEntity
            {
                TicketId = guid,
                ShowtimeId = showtimeId,
                Seats = seatsToReserve,
                CreatedTime = new DateTime(2024, 7, 2),
                Showtime = showtimeEntity
            };
            var result = await _ticketRepository.CreateAsync(ticketEntity, _dbFixtureShared.cancellationToken);

            TicketDto mappedResult = _mapper.Map<TicketDto>(result);

            var returnDto = await _ticketService.CreateTicketWithDelayAsync(ticketDto, nbrOfSeatsToreserv, _dbFixtureShared.cancellationToken);



            // Assert
            Assert.NotNull(showtimeEntity);
            Assert.NotNull(result);
            Assert.NotNull(showtimeEntity.Auditorium);
            Assert.Equal(showtimeId, showtimeEntity.ShowtimeId);
            Assert.Equal(10, showtimeEntity.Auditorium.AuditoriumId);
            //Assert.NotEmpty(seatsToReserve); //this not passed
            Assert.NotNull(mappedResult);
            Assert.NotNull(returnDto);


        }

        [Fact]
        public async void CreateTicketWithDelayAsync_NullShowtimeEntity_ShouldReturnNull()
        {
            int showtimeId = 4;
            var ticketDto = new TicketDto { ShowtimeId = showtimeId };

            var showtimeEntity = await _showtimesRepository.GetWithAuditAndTicketsAndSeats(ticketDto.ShowtimeId, _dbFixtureShared.cancellationToken);

            var result = await _ticketService.CreateTicketWithDelayAsync(ticketDto, 2, _dbFixtureShared.cancellationToken);
            Assert.Null(showtimeEntity);
            Assert.Null(result);
        }

        //[Fact]
        //public async void ConfirmPayementAsync()
        //{


        //}

    }
}

