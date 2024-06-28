using ApiApplication.CustomExceptions;
using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.IntegrationTests.FixtureClassesFirIntegration;
using ApiApplication.Models;
using ApiApplication.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using SharedFixtureTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static StackExchange.Redis.Role;

namespace ApiApplication.IntegrationTests
{
    public class ShowtimeServiceIntegrationTest : IClassFixture<DbFixtureIntegration>, IClassFixture<DependenciesTestFixture>
    {
        private readonly DbFixtureIntegration _dbFixtureShared;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimeService> _logger;
        private CinemaContext _dbContext;


        private readonly IAuditoriumsRepository _auditoriumsRepository;

        private readonly IShowtimesRepository _showtimesRepository;

        private readonly ILogger<ShowtimesRepository> repoLogger;

        private ShowtimeService _showtimeService;

        public ShowtimeServiceIntegrationTest(DbFixtureIntegration dbFixtureShared)
        {
            _dbFixtureShared = dbFixtureShared;
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

            _logger = loggerFactory.CreateLogger<ShowtimeService>();
            repoLogger = loggerFactory.CreateLogger<ShowtimesRepository>();

            _dbContext = _dbFixtureShared.context;

            _auditoriumsRepository = new AuditoriumsRepository(_dbContext);
            _showtimesRepository = new ShowtimesRepository(_dbContext, _mapper, repoLogger);

            _showtimeService = new ShowtimeService(_auditoriumsRepository,
                                                   _showtimesRepository,
                                                   _mapper,
                                                   _logger);

        }

        [Fact]
        public async void CreateShowtime_Success()
        {
            // Arrange
            var showtimeDto = new ShowtimeDto { AuditoriumId = 1 };

            // Act
            var result = await _showtimeService.CreateShowTime(showtimeDto, _dbFixtureShared.cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(showtimeDto.AuditoriumId, result.AuditoriumId);
            Assert.NotNull(result.Auditorium);
            Assert.Equal(showtimeDto.SessionDate, result.SessionDate);
        }

        [Fact]
        public async void CreateShowtime_NullShowtimeDto_ShouldThrowsArgNullExce()
        {
            ShowtimeDto? showtimeDto = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                                _showtimeService.CreateShowTime(showtimeDto, new CancellationToken()));


        }

        [Fact]
        public async void CreateShowtime_GetAuditById_ThrowExce()
        {
            var showtimedto = new ShowtimeDto { AuditoriumId = 3 };
            CancellationToken cancel = new CancellationToken();

            await Assert.ThrowsAsync<DataRetrieveException<AuditoriumEntity>>(()
                                    => _showtimeService.CreateShowTime(showtimedto, cancel));
        }

        [Fact]
        public async void CreateShowtime_ErrorMap()
        {

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ShowtimeEntity>(It.IsAny<ShowtimeDto>()))
                      .Throws(new MappingException<ShowtimeDto, ShowtimeEntity>("Mapping error"));

            var showtimeService = new ShowtimeService(_auditoriumsRepository,
                                                  _showtimesRepository,
                                                  mapperMock.Object,
                                                  _logger);

            var showtimeDto = new ShowtimeDto { AuditoriumId = 1 };
            var cancellationToken = CancellationToken.None;
            var auditoriumEntity = new AuditoriumEntity { auditoriumId = 1 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<MappingException<ShowtimeDto, ShowtimeEntity>>(() =>
                                                    showtimeService.CreateShowTime(showtimeDto, cancellationToken));
        }


        [Fact]
        public async void GetShowtimeByAuditoriumIdAndSessionDate_Success()
        {
            int auditoriumId = 1;
            DateTime sessionDate = DateTime.Now;
            CancellationToken cancellationToken = new CancellationToken();
            ShowtimeEntity showtimeEntity = new ShowtimeEntity
            {
                showtimeId = 3,
                SessionDate = sessionDate,
                AuditoriumId = auditoriumId,
                Movie = null,
                Tickets = null
            };
            _dbContext.Showtimes.Add(showtimeEntity);
            _dbContext.SaveChanges();

            var result = await _showtimeService.GetShowtimeByAuditoriumIdAndSessionDate(auditoriumId, sessionDate, cancellationToken);

            Assert.NotNull(result);
            // add gere others assert
        }

        [Fact]
        public async void GetShowtimeByAuditoriumIdAndSessionDate_ShowtimeEntityNull_ReturnNull()
        {
            int auditoriumId = 1;
            DateTime sessionDate = DateTime.Now;
            CancellationToken cancellationToken = new CancellationToken();


            var result = await _showtimeService.GetShowtimeByAuditoriumIdAndSessionDate(auditoriumId, sessionDate, cancellationToken);

            Assert.Null(result);

        }


        [Fact]
        public async void GetShowtimeByAuditoriumIdAndSessionDate_Throwsexception()
        {
            int auditoriumId = 1;
            DateTime sessionDate = DateTime.Now;

            var showtimesRepository = new Mock<IShowtimesRepository>();
                showtimesRepository.Setup(repo => repo.GetByAuditoriumIdAndSessionDateAsync
                                                (auditoriumId, sessionDate, _dbFixtureShared.cancellationToken))
                                    .ThrowsAsync(new ArgumentException($"showtime with auditorium Id {auditoriumId} was not found."));

             var showtimeService = new ShowtimeService(_auditoriumsRepository,
                                                  showtimesRepository.Object,
                                                  _mapper,
                                                  _logger);
            var result = await Assert.ThrowsAsync<ArgumentException>(() => 
                                                    showtimeService.GetShowtimeByAuditoriumIdAndSessionDate(auditoriumId,
                                                                                                            sessionDate,
                                                                                                            _dbFixtureShared.cancellationToken));
            Assert.Equal($"showtime with auditorium Id {auditoriumId} was not found.", result.Message);
        }

        [Fact]
        public async void GetShowtimeByAuditoriumIdAndSessionDate_ThrowsexceptionMapping()
        {
            int auditoriumId = 1;
            DateTime sessionDate = DateTime.Now;

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ShowtimeDto>(It.IsAny<ShowtimeEntity>()))
                      .Throws(new MappingException<ShowtimeEntity, ShowtimeDto>("Mapping error"));

            var showtimeService = new ShowtimeService(_auditoriumsRepository,
                                                  _showtimesRepository,
                                                  mapperMock.Object,
                                                  _logger);

            ShowtimeEntity showtimeEntity = new ShowtimeEntity
            {
                showtimeId = 3,
                SessionDate = sessionDate,
                AuditoriumId = auditoriumId,
                Movie = null,
                Tickets = null
            };
            _dbContext.Showtimes.Add(showtimeEntity);
            _dbContext.SaveChanges();


            var exception = await Assert.ThrowsAsync<MappingException<ShowtimeEntity, ShowtimeDto>>(() =>
                                                    showtimeService.GetShowtimeByAuditoriumIdAndSessionDate(auditoriumId,
                                                                                                            sessionDate,
                                                                                                            _dbFixtureShared.cancellationToken));

        }
    }
}

