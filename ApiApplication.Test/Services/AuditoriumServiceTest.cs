using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiApplication.Database.Repositories;
using ApiApplication.Database;
using ApiApplication.Models;
using ApiApplication.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using ApiApplication.Database.Repositories.Abstractions;
using AutoMapper;

namespace ApiApplication.Test.Services
{
    public class AuditoriumServiceTest
    {
        // I mock the IAuditoriumsRepository interface
        private readonly Mock<IAuditoriumsRepository> _mockAuditoriumRepository;
        private readonly AuditoriumService _AuditoriumService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CinemaContext _inMemoryCinemaContext;

        public AuditoriumServiceTest()
        {
            _mockAuditoriumRepository = new Mock<IAuditoriumsRepository>();
            _mockMapper = new Mock<IMapper>();
            // configure an in-memory Db context
            var option = new DbContextOptionsBuilder<CinemaContext>().UseInMemoryDatabase(databaseName: "TestDB").Options;
            _inMemoryCinemaContext = new CinemaContext(option);

            _AuditoriumService = new AuditoriumService(_inMemoryCinemaContext, _mockMapper.Object, _mockAuditoriumRepository.Object);
        }

        [Fact]
        public async Task AuditoriumExistAsyncTest_ReturnsTrue_WhenExist()
        {

            // Arrange
            var auditoriumId = 1;

            //I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            _mockAuditoriumRepository.Setup(repo => repo.AuditoriumExistAsync(auditoriumId)).ReturnsAsync(true);

            // Act
            bool result = await _AuditoriumService.AuditoriumExistAsync(auditoriumId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AuditoriumExistAsyncTest_ReturnsFalse_WhenNotExist()
        {
            var auditoriumId = 10;

            //I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            _mockAuditoriumRepository.Setup(repo => repo.AuditoriumExistAsync(auditoriumId)).ReturnsAsync(false);

            // Act
            bool result = await _AuditoriumService.AuditoriumExistAsync(auditoriumId);

            // Assert
            Assert.False(result);
        }
    }
}
