using ApiApplication.Controllers;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiApplication.Test.ControllerTests
{
    public class ShowtimesControllerTest
    {
        private readonly Mock<IShowtimeService> _mockShowtimeService;
        private readonly Mock<IMovieService> _mockMovieService;
        private readonly Mock<IAuditoriumService> _mockAuditoriumService;
        private readonly Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger<ShowtimesController>> _mockLogger;

        private readonly ShowtimesController _controller;

        public ShowtimesControllerTest()
        {
            _mockShowtimeService = new Mock<IShowtimeService>();
            _mockMovieService = new Mock<IMovieService>();
            _mockAuditoriumService = new Mock<IAuditoriumService>();
            _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ShowtimesController>>();

            _controller = new ShowtimesController(_mockShowtimeService.Object,
                                                  _mockAuditoriumService.Object,
                                                  _mockMovieService.Object,
                                                  _mockLogger.Object,
                                                  _mockAuditoriumsRepository.Object,
                                                  _mapper.Object);
        }

        [Fact]
        public async Task CreateShowtime_InvalidAuditoId_ReturnBadRequest()
        {
            // Arrange
            var auditId = -1;
            string movieId = "x";
            DateTime dateTime = DateTime.UtcNow;
            CancellationToken cancellationToken = new CancellationToken();


            // Act
            var result = await _controller.CreateShowtime(movieId,auditId, dateTime, cancellationToken);

            // Assert
           var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result); // to verify that the response is BadRequest
            Assert.Equal($"Invalid auditoriumId {auditId}", badRequest.Value);
        }


        [Fact]
        public async Task CreateShowtime_ReturnOk()
        {
            // Arrange
            var auditId = 1;
            string movieId = "x";
            DateTime dateTime = DateTime.Now.AddDays(1);
            CancellationToken cancellationToken = new CancellationToken();
            MovieDto movieDto = new MovieDto
            {
                movieId = movieId,
            };

            ShowtimeDto showtimeDto = new ShowtimeDto
            {
                AuditoriumId = auditId,
                Movie = movieDto,
                SessionDate = dateTime,
                showtimeId = 3
                
            };

            _mockAuditoriumService.Setup(s => s.AuditoriumExistAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockShowtimeService.Setup(s => s.ShowtimeExistAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(false);
            _mockMovieService.Setup(s => s.GetMovieById(It.IsAny<string>())).ReturnsAsync(movieDto);

            _mockShowtimeService.Setup(service => service.CreateShowTime(It.IsAny<ShowtimeDto>(), cancellationToken)).ReturnsAsync(showtimeDto);

            //// Mock HttpContext and HttpRequest
            //var mockHttpContext = new Mock<HttpContext>();
            //var mockHttpRequest = new Mock<HttpRequest>();
            //var mockHttpResponse = new Mock<HttpResponse>();

            //mockHttpContext.SetupGet(x => x.Request).Returns(mockHttpRequest.Object);
            //mockHttpContext.SetupGet(x => x.Response).Returns(mockHttpResponse.Object);

            //// Mock the GetDisplayUrl extension method
            //var mockUrlHelper = new Mock<IUrlHelper>();
            //mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
            //    .Returns("http://test.com/api/showtimes/1");
            //_controller.Url = mockUrlHelper.Object;


            // Act
            var result = await _controller.CreateShowtime(movieId, auditId, dateTime, cancellationToken); 

            // Assert
            
            var actionResult = Assert.IsType<ActionResult<ShowtimeDto>>(result);
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
            Assert.Equal("GetShowtimeWithMovie", createdAtRouteResult.RouteName);
            Assert.Equal(showtimeDto.showtimeId, createdAtRouteResult?.RouteValues?["id"]);
            Assert.Equal(showtimeDto, createdAtRouteResult?.Value);

        }


        [Fact]
        public async Task GetShowtimeWithMovieById_ReturnsOkResult_WhenShowtimeExists()
        {
            // Arrange
            var id = 1;
            CancellationToken cancellationToken = new CancellationToken();
            ShowtimeDto showtimeDto = new ShowtimeDto { showtimeId = id };

            _mockShowtimeService.Setup(service => service.GetShowtimeWithMovieById(id, cancellationToken)).ReturnsAsync(showtimeDto);

            // Act
            var result = await _controller.GetShowtimeWithMovie(id, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // to verify that the response is OkObjectResult

            var returnValue = Assert.IsType<ShowtimeDto>(okResult.Value);// to verify that the response value is of type ShowtimeDto
            Assert.Equal(id, returnValue.showtimeId);
           
        }

        [Fact]
        public async Task GetShowtimeWithMovieById_ReturnsNotFound_WhenShowtimeNotExists()
        {
            // Arrange
            var id = 1;
            CancellationToken cancellationToken = new CancellationToken();
            ShowtimeDto showtimeDto = new ShowtimeDto { showtimeId = id };

            _mockShowtimeService.Setup(service => service.GetShowtimeWithMovieById(id, cancellationToken)).ReturnsAsync((ShowtimeDto?)null);

            // Act
            var result = await _controller.GetShowtimeWithMovie(id, cancellationToken);

            // Assert
           
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
