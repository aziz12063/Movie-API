using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.CustomExceptions;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiApplication.Test.Services
{
    public class ShowtimeServiceTest
    {
        private readonly Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        //private readonly Mock<IMovieService> _movieServiceMock;
        private readonly Mock<IShowtimesRepository> _mockShowtimesRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ShowtimeService>> _loggerMock;

        private ShowtimeService _showtimeService;

        public ShowtimeServiceTest()
        {
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
           // _movieServiceMock = new Mock<IMovieService>();
            _mockShowtimesRepository = new Mock<IShowtimesRepository>();
            _mockMapper = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ShowtimeService>>();

            _showtimeService = new ShowtimeService(_mockAuditoriumsRepository.Object,
                                                    //_movieServiceMock.Object,
                                                    _mockShowtimesRepository.Object,
                                                    _mockMapper.Object,
                                                    _loggerMock.Object);

        }

       



        [Fact]
        public async void CreateShowtime_Success()
        {
            // Arrange
            var showtimeDto = new ShowtimeDto { AuditoriumId = 1 };
            var cancellationToken = CancellationToken.None;
            var auditoriumEntity = new AuditoriumEntity { auditoriumId = 1 };
            var showtimeEntity = new ShowtimeEntity();
            var createdShowtimeDto = new ShowtimeDto { AuditoriumId = 1 };

            showtimeEntity.Auditorium = auditoriumEntity;


            _mockAuditoriumsRepository.Setup(repo =>
                repo.GetByIdWithSeatsAndShowtimesAsync(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(auditoriumEntity);


            _mockMapper.Setup(mapper =>
                mapper.Map<ShowtimeEntity>(It.IsAny<ShowtimeDto>()))
                .Returns(showtimeEntity);


            _mockShowtimesRepository.Setup(repo =>
                repo.CreateShowtime(showtimeEntity, cancellationToken))
                .ReturnsAsync(createdShowtimeDto);




            // Act
            var result = await _showtimeService.CreateShowTime(showtimeDto, cancellationToken);

            // Assert
            Assert.Equal(createdShowtimeDto, result);
        }

        [Fact]
        public async void CreateShowtime_NullShowtimeDto_ShouldThrowsArgNullExce()
        {
            ShowtimeDto? showtimeDto = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _showtimeService.CreateShowTime(showtimeDto, new CancellationToken()));

            //_loggerMock.Verify(logger =>

            //    logger.LogError("The showtime is null"),
            //     Times.Once);

            _loggerMock.Verify(
            // We can use VerifyLog to check for a specific log message and LogLevel
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("The showtime is null")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
        }

        [Fact]
        public async void CreateShowtime_GetAuditRepoById_ThrowExce()
        {
            var showtimedto = new ShowtimeDto {AuditoriumId = 1 };
            CancellationToken cancel = new CancellationToken();
            _mockAuditoriumsRepository.Setup(x => x.GetByIdWithSeatsAndShowtimesAsync(It.IsAny<int>(), cancel))
                                                                                      .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<DataRetrieveException<AuditoriumEntity>>(() => _showtimeService.CreateShowTime(showtimedto, cancel));
        }

        [Fact]
        public async void CreateShowtime_ErrorMap()
        {
            var showtimeDto = new ShowtimeDto { AuditoriumId = 1 };
            var cancellationToken = CancellationToken.None;
            var auditoriumEntity = new AuditoriumEntity { auditoriumId = 1 };

            _mockAuditoriumsRepository.Setup(repo =>
                                            repo.GetByIdWithSeatsAndShowtimesAsync(It.IsAny<int>(), cancellationToken))
                                            .ReturnsAsync(auditoriumEntity);


            _mockMapper.Setup(mapper => mapper.Map<ShowtimeEntity>(It.IsAny<ShowtimeDto>())).Throws(new Exception("Mapping error"));
            

            // Act & Assert
            var exception = await Assert.ThrowsAsync<MappingException<ShowtimeDto, ShowtimeEntity>>(() =>
                                                    _showtimeService.CreateShowTime(showtimeDto, cancellationToken));
        }

        [Fact]
        public async void GetShowtimeByAuditoriumIdAndSessionDate_ShowtimeEntityNull_ReturnNull()
        {
            int auditoriumId = 1;
            DateTime sessionDate = DateTime.Now;
            CancellationToken cancellationToken = new CancellationToken();
            ShowtimeEntity? showtimeEntity = null;

            _mockShowtimesRepository.Setup(x => x.GetByAuditoriumIdAndSessionDateAsync(It.IsAny<int>(), It.IsAny<DateTime>(), cancellationToken))
                                                                                       .ReturnsAsync(showtimeEntity);
            var result = await _showtimeService.GetShowtimeByAuditoriumIdAndSessionDate(auditoriumId, sessionDate, cancellationToken);

            Assert.Null(result);

        }
    }
}
