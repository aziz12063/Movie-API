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
using NuGet.Frameworks;
using Moq;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using Google.Protobuf.WellKnownTypes;
using AutoMapper;

namespace ApiApplication.Test.Services
{
    public class AuditoriumServiceTest
    {
        [Fact]
        public async Task AuditoriumExistAsyncTest_ReturnsTrue_WhenExist()
        {
            // 1- I mock the IAuditoriumsRepository interface
            // 2- I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            // 3- Then, I create an instance of AuditoriumService with the mocked repository and call the method to verify its behavior.

            // Arrange
            var options = new DbContextOptionsBuilder<CinemaContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            var context = new CinemaContext(options); // Provide options to the context constructor

            var auditoriumId = 10;

            // I mock the IAuditoriumsRepository interface
            var mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();

            //I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            mockAuditoriumsRepository.Setup(repo => repo.AuditoriumExistAsync(auditoriumId)).ReturnsAsync(true);

            var service = new AuditoriumService(context, new Mock<IMapper>().Object, mockAuditoriumsRepository.Object);

            // Act
            var result = await service.AuditoriumExistAsync(auditoriumId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AuditoriumExistAsyncTest_ReturnsFalse_WhenNotExist()
        {
            // 1- I mock the IAuditoriumsRepository interface
            // 2- I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            // 3- Then, I create an instance of AuditoriumService with the mocked repository and call the method to verify its behavior.

            // Arrange
            var options = new DbContextOptionsBuilder<CinemaContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            var context = new CinemaContext(options); // Provide options to the context constructor

            var auditoriumId = 10;

            // I mock the IAuditoriumsRepository interface
            var mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();

            //I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            mockAuditoriumsRepository.Setup(repo => repo.AuditoriumExistAsync(auditoriumId)).ReturnsAsync(false);

            var service = new AuditoriumService(context, new Mock<IMapper>().Object, mockAuditoriumsRepository.Object);

            // Act
            var result = await service.AuditoriumExistAsync(auditoriumId);

            // Assert
            Assert.False(result);
        }
    }
}
