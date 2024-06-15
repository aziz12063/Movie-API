using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ApiApplication.Cache;
using ApiApplication.Models;
using ApiApplication.ProvidedApi.Entities;
using ApiApplication.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace ApiApplication.Test.Services
{
    public class MovieServiceTest
    {
        // Mock Dependencies
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MovieService>> _mockLogger;
        private readonly Mock<IResponseCacheService> _mockResponseCacheService;
        private readonly MovieService _movieService;

        public MovieServiceTest( )
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:7172/v1/movies")
            };
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<MovieService>>();
            _mockResponseCacheService = new Mock<IResponseCacheService>();
            _movieService = new MovieService( _httpClient, _mockMapper.Object, _mockLogger.Object, _mockResponseCacheService.Object );
        }


        [Fact]
        public async Task GetMovieById_ReturnMovie_WhenApiCallIsSuccessfull_ApplicationJson()
        {
            // Arrange
            var movieId = "1";
            var movieApiEntity = new MoviesApiEntity { Id = movieId, Title = "TestTitle", ImDbRating = "tt1234567" };
            var movieDto = new MovieDto { movieId = movieId, Title = "TestTitle", ImdbId = "tt1234567", ReleaseDate = DateTime.Now };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(movieApiEntity), Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                                                                                 ItExpr.IsAny<HttpRequestMessage>(),
                                                                                 ItExpr.IsAny<CancellationToken>()
            )
                                                .ReturnsAsync(responseMessage);


            _mockMapper.Setup(m => m.Map<MovieDto>(It.IsAny<MoviesApiEntity>())).Returns(movieDto);

            // Act
            var result = await _movieService.GetMovieById(movieId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(movieId, result.movieId);
            Assert.Equal("TestTitle", result.Title);

        }

        [Fact]
        public async Task GetMovieById_ReturnMovie_WhenApiCallIsSuccessfull_ApplicationXml()
        {
            // Arrange
            var movieId = "1";
            var movieApiEntity = new MoviesApiEntity { Id = movieId, Title = "TestTitle", ImDbRating = "tt1234567" };
            var movieDto = new MovieDto { movieId = movieId, Title = "TestTitle", ImdbId = "tt1234567", ReleaseDate = DateTime.Now };

            _mockMapper.Setup(m => m.Map<MovieDto>(It.IsAny<MoviesApiEntity>())).Returns(movieDto);

            var xmlResponse = "<MoviesApiEntity><MovieId>1</MovieId><Title>TestTitle</Title>...</MoviesApiEntity>";

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xmlResponse)
            };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                                                                                 ItExpr.IsAny<HttpRequestMessage>(),
                                                                                 ItExpr.IsAny<CancellationToken>()
            )
                                                .ReturnsAsync(responseMessage);


            // Act
            var result = await _movieService.GetMovieById(movieId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(movieId, result.movieId);
            Assert.Equal("TestTitle", result.Title);

        }

        [Fact]
        public async Task GetMovieById_ReturnsMovie_FromCache_WhenApiCallFails()
        {
            // Arrange
            var movieId = "1";
            var movieDto = new MovieDto { movieId = movieId, Title = "Cached Movie", ImdbId = "tt1234567", ReleaseDate = DateTime.Now };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            _mockResponseCacheService.Setup(c => c.GetCachedResponseAsync(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(movieDto));

            // Act
            var result = await _movieService.GetMovieById(movieId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movieId, result.movieId);
            Assert.Equal("Cached Movie", result.Title);
        }

        [Fact]
        public async Task GetMovieById_ThrowsException_WhenMovieNotFoundInApiOrCache()
        {
            // Arrange
            var movieId = "1";

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            _mockResponseCacheService.Setup(c => c.GetCachedResponseAsync(It.IsAny<string>())).ReturnsAsync(string.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _movieService.GetMovieById(movieId));
        }

    }
}
