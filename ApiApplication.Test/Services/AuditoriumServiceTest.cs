using Moq;
using ApiApplication.Test.TestFixture;
using Xunit;

namespace ApiApplication.Test.Services
{
    public class AuditoriumServiceTest : IClassFixture<AuditoriumServiceFixture>
    {
        private readonly AuditoriumServiceFixture _fixture;

        public AuditoriumServiceTest(AuditoriumServiceFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public async Task AuditoriumExistAsyncTest_ReturnsTrue_WhenExist()
        {
            // Arrange
            var auditoriumId = 1;

            //I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            _fixture._mockAuditoriumRepository.Setup(repo => repo.AuditoriumExistAsync(auditoriumId)).ReturnsAsync(true);

            // Act
            bool result = await _fixture._AuditoriumService.AuditoriumExistAsync(auditoriumId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AuditoriumExistAsyncTest_ReturnsFalse_WhenNotExist()
        {
            var auditoriumId = 10;

            //I set up its behavior to return a boolean value representing whether an auditorium with the given ID exists or not.
            _fixture._mockAuditoriumRepository.Setup(repo => repo.AuditoriumExistAsync(auditoriumId)).ReturnsAsync(false);

            // Act
            bool result = await _fixture._AuditoriumService.AuditoriumExistAsync(auditoriumId);

            // Assert
            Assert.False(result);
        }
    }
}
