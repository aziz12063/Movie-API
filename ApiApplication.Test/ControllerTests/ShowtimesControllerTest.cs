using ApiApplication.Controllers;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
