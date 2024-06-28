using ApiApplication.Database;
using ApiApplication.Database.Repositories;
using ApiApplication.Test.TestFixture;
using Xunit;

namespace ApiApplication.Test.RepositoryTest
{
    [Collection("DB collection")]
    public class AuditoriulRepositoryTest
    {
        private readonly DataBaseFixture _fixture;

        public AuditoriulRepositoryTest(DataBaseFixture fixture)
        {
            _fixture = fixture;
        }

       
        
        [Fact]
        public async void GetByIdWithSeatsAndShowtimesAsync_AuditNotExist_ShouldReturnNull()
        {
            int id = 3;
            
            using(var context = new CinemaContext(_fixture._dbContextOptions))
            {
                var repo = new AuditoriumsRepository(context);

                // Act
                var result = await repo.GetByIdWithSeatsAndShowtimesAsync(id, _fixture.cancellationToken);

                // Assert
                Assert.Null(result);
                                
            }
        }

        [Theory]
        //[InlineData(id, expectedAuditoriumId)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public async void GetByIdWithSeatsAndShowtimesAsync_DataDrivenTest(int id, int expectedAuditoriumId)
        {
            using (var context = new CinemaContext(_fixture._dbContextOptions))
            {
                var repo = new AuditoriumsRepository(context);

                // Act
                var result = await repo.GetByIdWithSeatsAndShowtimesAsync(id, _fixture.cancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expectedAuditoriumId, result.auditoriumId);
            }
        }

        


    }
}
