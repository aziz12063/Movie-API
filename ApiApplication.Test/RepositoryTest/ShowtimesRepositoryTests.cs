using ApiApplication.Database;
using ApiApplication.Test.TestFixture;
using ApiApplication.Database.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;


namespace ApiApplication.Test.RepositoryTest
{
    public class ShowtimesRepositoryTests : IClassFixture<DataBaseFixture>
    {
        private readonly DataBaseFixture _fixture;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger<ShowtimesRepository>> _logger;
        private readonly CinemaContext context;
        private readonly ShowtimesRepository repo;

        public ShowtimesRepositoryTests(DataBaseFixture fixture)
        {
            _fixture = fixture;
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<ShowtimesRepository>>();
            context = new CinemaContext(_fixture._dbContextOptions);
            repo = new ShowtimesRepository(context, _mapper.Object, _logger.Object);
        }

        [Fact]
        public async void GetWithMoviesByIdAsync_ShowExist()
        {
            int id = 1;

                // Act
                var result = await repo.GetWithMoviesByIdAsync(id, _fixture.cancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(id, result.showtimeId);

        }

        [Fact]
        public async void GetWithMoviesByIdAsync_ShowNotExist()
        {
            int id = 3;

            // Act
            var result = await repo.GetWithMoviesByIdAsync(id, _fixture.cancellationToken);

            // Assert
            Assert.Null(result);
        }

       

    }

    
}
