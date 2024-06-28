using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database.Repositories;
using ApiApplication.Database;
using ApiApplication.Services;
using FluentAssertions;
using Xunit;
using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using SharedFixtureTest;

namespace ApiApplication.IntegrationTests
{
    //[Collection("SharedDB")]
    public class AuditoriumServiceIntegrationTest : IClassFixture<DbFixtureShared>
    {
        private  AuditoriumService _service;
       // private  CinemaContext _dbContext;
        private  IAuditoriumsRepository _auditoriumsRepository;
        private  DbFixtureShared _fixture;

        public AuditoriumServiceIntegrationTest(DbFixtureShared fixture)
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
            var auditorium = new AuditoriumEntity { auditoriumId = 1 };
           // _dbContext.Auditoriums.Add(auditorium);
            //await _dbContext.SaveChangesAsync();

            // Act
            bool result = await _service.AuditoriumExistAsync(auditorium.auditoriumId);

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
