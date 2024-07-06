using ApiApplication.Database;
using ApiApplication.Test.TestFixture;
using ApiApplication.Database.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;


namespace ApiApplication.Test.RepositoryTest
{
    [Collection("DB collection")]
    public class ShowtimesRepositoryTests //: IClassFixture<DataBaseFixture>
    {
        private readonly DataBaseFixture _fixture;
        private readonly Mock<IMapper> _mapper;
        private readonly CinemaContext context;
        private readonly ShowtimesRepository repo;

        public ShowtimesRepositoryTests(DataBaseFixture fixture)
        {
            _fixture = fixture;
            _mapper = new Mock<IMapper>();
            context = new CinemaContext(_fixture._dbContextOptions);
            repo = new ShowtimesRepository(context, _mapper.Object);
        }

        [Fact]
        public async void GetWithMoviesByIdAsync_ShowExist()
        {
            int id = 1;

                // Act
                var result = await repo.GetWithMoviesByIdAsync(id, _fixture.cancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(id, result.ShowtimeId);
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
