using ApiApplication.Controllers;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiApplication.Test.ControllerTests
{
    public class TicketsControllerTest
    {
        private readonly Mock<ITicketService> _mockTicketService;
        private readonly Mock<IShowtimesRepository> _mockShowtimesRepository;
        private readonly Mock<ISeatService> _mockSeatService;
        private readonly Mock<ITicketsRepository> _mockTicketsRepository;
        private readonly Mock<IAuditoriumsRepository> _mockAuditoriumsRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger<TicketsController>> _mockLogger;

        private readonly TicketsController _controller;

        public TicketsControllerTest()
        {
            _mockShowtimesRepository = new Mock<IShowtimesRepository>();
            _mockTicketService = new Mock<ITicketService>();
            _mockSeatService = new Mock<ISeatService>();
            _mockTicketsRepository = new Mock<ITicketsRepository>();
             _mockAuditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _mapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TicketsController>>();

            _controller = new TicketsController(_mockTicketService.Object,
                                                  _mockLogger.Object,
                                                  _mockShowtimesRepository.Object,
                                                  _mapper.Object,
                                                  _mockAuditoriumsRepository.Object,
                                                  _mockSeatService.Object,
                                                  _mockTicketsRepository.Object);
        }

        [Fact]
        public async Task CreateTicket_InvalidshowtimeId_ReturnBadRequest()
        {
            // Arrange
            var showtimeId = -1;
            int nbrOfSeat = 5;
            CancellationToken cancellationToken = new CancellationToken();

            // Act
            var result = await _controller.CreateTicket(showtimeId,nbrOfSeat, cancellationToken);

            // Assert
            var badRequest = Assert.IsType<BadRequestResult>(result.Result); // to verify that the response is BadRequest
            
        }


        [Fact]
        public async Task CreateTicket_ReturnOk()
        {
            // Arrange
            var showtimeId = 1;
            int nbrOfSeats = 2;
            DateTime dateTime = DateTime.Now;
            CancellationToken cancellationToken = new CancellationToken();
          
            TicketDto ticketDto = new TicketDto
            {
                ShowtimeId = showtimeId,
                CreatedTime = dateTime,
            };

            _mockTicketService.Setup(s => s.CreateTicketWithDelayAsync(It.IsAny<TicketDto>(),
                                                                       It.IsAny<int>(),
                                                                       cancellationToken))
                                            .ReturnsAsync(ticketDto);

            // Act
            var result = await _controller.CreateTicket(showtimeId, nbrOfSeats, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // to verify that the response is Okresult

            var returnedTicketDto = okResult.Value as TicketDto; // cast the return to TicketDto 

            Assert.NotNull(returnedTicketDto);
            Assert.Equal(ticketDto, returnedTicketDto);
        }



    }
}
