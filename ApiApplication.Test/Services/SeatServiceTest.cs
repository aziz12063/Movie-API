using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Services;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;


namespace ApiApplication.Test.Services
{
    public class SeatServiceTest
    {

        private readonly Mock<ILogger<SeatService>> _mockLogger;
        private SeatService _seatService;

        public SeatServiceTest()
        {
            _mockLogger = new Mock<ILogger<SeatService>>();
            _seatService = new(_mockLogger.Object);
        }


        [Fact]
        public async Task FindSeatsContiguous_ReturnListOfSeats_IfFound()
        {
            // Arrange
            IEnumerable<SeatEntity> availableSeats = new List<SeatEntity>
            {
                new SeatEntity { Row = 1, SeatNumber = 1 },
                new SeatEntity { Row = 1, SeatNumber = 2 },
                new SeatEntity { Row = 1, SeatNumber = 3 },
                new SeatEntity { Row = 2, SeatNumber = 1 },
                new SeatEntity { Row = 2, SeatNumber = 2 },
                new SeatEntity { Row = 2, SeatNumber = 3 },
            };


            int nbrOfSeatsToReserve = 2;
            CancellationToken cancel = new();

            // Act
            var result = await _seatService.FindSeatsContiguous(availableSeats, nbrOfSeatsToReserve, cancel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].SeatNumber);
            Assert.Equal(1, result[0].Row);

            Assert.Equal(2, result[1].SeatNumber);
            Assert.Equal(1, result[1].Row);
     


        }

        [Fact]
        public async Task FindSeatsContiguous_ReturnNull_IfNotFound()
        {
            // Arrange
            IEnumerable<SeatEntity> availableSeats = new List<SeatEntity>
            {
                new SeatEntity { Row = 1, SeatNumber = 1 },
                new SeatEntity { Row = 1, SeatNumber = 2 },
                new SeatEntity { Row = 1, SeatNumber = 5 },
                new SeatEntity { Row = 2, SeatNumber = 1 },
                new SeatEntity { Row = 2, SeatNumber = 6 },
                new SeatEntity { Row = 2, SeatNumber = 3 },
            };


            int nbrOfSeatsToReserve = 3;
            CancellationToken cancel = new();

          
            // Act
            var result = await _seatService.FindSeatsContiguous(availableSeats, nbrOfSeatsToReserve, cancel);

            // Assert
            Assert.Null(result);

            // i want to test the log message too
           

        }


        [Fact]
        public void IsContiguous_ReternTrueIfContiguous()
        {
            List<SeatEntity> seats = new List<SeatEntity>
            {
                new SeatEntity { Row = 1, SeatNumber = 1 },
                new SeatEntity { Row = 1, SeatNumber = 2 },
                new SeatEntity { Row = 1, SeatNumber = 3 },
               
            };

            var result = _seatService.IsContiguous(seats);

            Assert.True(result);
        }


        [Fact]
        public void IsContiguous_ReturnFalseIfNotContiguousSeatNumber()
        {
            List<SeatEntity> seats = new List<SeatEntity>
            {
                new SeatEntity { Row = 1, SeatNumber = 1 },
                new SeatEntity { Row = 1, SeatNumber = 3 },
                new SeatEntity { Row = 1, SeatNumber = 4 },

            };

            var result = _seatService.IsContiguous(seats);

            Assert.False(result);
        }

        [Fact]
        public void IsContiguous_ReturnFalseIfNotContiguous_Row()
        {
            List<SeatEntity> seats = new List<SeatEntity>
            {
                new SeatEntity { Row = 1, SeatNumber = 1 },
                new SeatEntity { Row = 1, SeatNumber = 2 },
                new SeatEntity { Row = 2, SeatNumber = 3 },

            };

            var result = _seatService.IsContiguous(seats);

            Assert.False(result);
        }
    }
}
