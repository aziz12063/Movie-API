using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database.Repositories;
using ApiApplication.Database;
using ApiApplication.IntegrationTests.FixtureClassesFirIntegration;
using ApiApplication.Services.Interfaces;
using ApiApplication.Services;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ApiApplication.Controllers;
using ApiApplication.Cache;
using Microsoft.Extensions.DependencyModel;
using Moq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using static StackExchange.Redis.Role;
using ApiApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.IntegrationTests
{
    public class ShowtimeControllerIntegrationTest : IClassFixture<DbFixtureIntegration>
    {
        // dependency showtimeController
        private readonly IShowtimeService _showtimeService;
        private readonly Mock<IMovieService> _mockMovieService;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimesController> _loggerShowtimesController;

        // dependency ShowtimeService
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly ILogger<ShowtimeService> _loggerShowtimeService;

        private readonly ILogger<ShowtimesRepository> _loggerShowtimeRepo;

        private DbFixtureIntegration _dbFixtureShared;
        private CinemaContext _dbContext;
        private readonly ShowtimesController _showtimesController;



        public ShowtimeControllerIntegrationTest(DbFixtureIntegration dbFixtureShared)
        {
            _dbFixtureShared = dbFixtureShared;
            _dbContext = _dbFixtureShared.context;

            _mockMovieService = new Mock<IMovieService>();

            _mapper = _dbFixtureShared._mapper;
            var loggerFactory = _dbFixtureShared._loggerFactory;

            _loggerShowtimeRepo = loggerFactory.CreateLogger<ShowtimesRepository>();
            _loggerShowtimesController = loggerFactory.CreateLogger<ShowtimesController>();
            _loggerShowtimeService = loggerFactory.CreateLogger<ShowtimeService>();


            _auditoriumsRepository = new AuditoriumsRepository(_dbContext);
            _showtimesRepository = new ShowtimesRepository(_dbContext,
                                                           _mapper,
                                                           _loggerShowtimeRepo);

            _showtimeService = new ShowtimeService(_auditoriumsRepository,
                                                   _showtimesRepository,
                                                   _mapper,
                                                   _loggerShowtimeService);

            _auditoriumService = new AuditoriumService(_auditoriumsRepository);

            _showtimesController = new ShowtimesController(_showtimeService,
                                                           _auditoriumService,
                                                           _mockMovieService.Object,
                                                           _loggerShowtimesController,
                                                           _auditoriumsRepository,
                                                           _mapper);
        }

        [Fact]
        public async void CreateShowtime_Success_ReturnShowtimeDto()
        {
            string movieId = "1";
            int auditoriumId = 10;
            DateTime sessionDate = DateTime.Now.AddDays(1);
            CancellationToken cancellationToken = new CancellationToken();

            MovieDto movieDto = new MovieDto()
            {
                movieId = movieId,
                Title = "test"
            };

            ShowtimeDto showtimeDto = new ShowtimeDto
            {
                AuditoriumId = auditoriumId,
                Movie = movieDto,
                SessionDate = sessionDate,
                showtimeId = 11

            };

            _mockMovieService.Setup(s => s.GetMovieById(It.IsAny<string>())).ReturnsAsync(movieDto);

            var result = await _showtimesController.CreateShowtime(movieId, auditoriumId, sessionDate, cancellationToken);

            Assert.NotNull(result);
            var actionResult = Assert.IsType<ActionResult<ShowtimeDto>>(result);
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
            Assert.Equal("GetShowtimeWithMovie", createdAtRouteResult.RouteName);
            Assert.Equal(showtimeDto.showtimeId, createdAtRouteResult.RouteValues["id"]);
            //Assert.Equal(showtimeDto, createdAtRouteResult.Value);
        }

        [Fact]
        public async Task GetShowtimeWithMovieById_ReturnsOkResult_WhenShowtimeExists()
        {
            // Arrange
            var id = 1;
            CancellationToken cancellationToken = new CancellationToken();

            // Act
            var result = await _showtimesController.GetShowtimeWithMovie(id, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // to verify that the response is OkObjectResult

            var returnValue = Assert.IsType<ShowtimeDto>(okResult.Value);// to verify that the response value is of type ShowtimeDto
            Assert.Equal(id, returnValue.showtimeId);

        }
    }

}

