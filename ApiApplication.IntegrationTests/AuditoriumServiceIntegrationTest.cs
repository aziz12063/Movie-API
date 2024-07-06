using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database.Repositories;
using ApiApplication.Database;
using ApiApplication.Services;
using FluentAssertions;
using Xunit;
using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using SharedFixtureTest;
using ApiApplication.IntegrationTests.FixtureClassesFirIntegration;

namespace ApiApplication.IntegrationTests
{
    [Collection("DB collection")]
    public class AuditoriumServiceIntegrationTest //: IClassFixture<DbFixtureShared>
    {
        private readonly  AuditoriumService _service;
       // private  CinemaContext _dbContext;
        private readonly  IAuditoriumsRepository _auditoriumsRepository;
        private readonly DbFixtureIntegration _fixture;

        public AuditoriumServiceIntegrationTest(DbFixtureIntegration fixture)
        {
            //var options = new DbContextOptionsBuilder<CinemaContext>()
            //                .UseInMemoryDatabase(databaseName: "TestCinemaDatabase")
            //                .Options;
            _fixture = fixture;
            //_dbContext = new CinemaContext(_fixture._dbContextOptions);

            _auditoriumsRepository = new AuditoriumsRepository(_fixture.context);
            _service = new AuditoriumService(_auditoriumsRepository);
        }


        [Fact]
        public async Task AuditoriumExistAsync_ShouldReturnTrue_WhenAuditoriumExists()
        {
            // Arrange
            //InitializeTest();
            var auditorium = new AuditoriumEntity { AuditoriumId = 1 };
           // _dbContext.Auditoriums.Add(auditorium);
            //await _dbContext.SaveChangesAsync();

            // Act
            bool result = await _service.AuditoriumExistAsync(auditorium.AuditoriumId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AuditoriumExistAsync_ShouldReturnFalse_WhenAuditoriumDoesNotExist()
        {
            // Act
            bool result = await _service.AuditoriumExistAsync(999); // Non-existent ID

            // Assert
            result.Should().BeFalse();
        }

       
    }
}
