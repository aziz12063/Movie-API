using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class TicketService : ITicketService
    {

        private readonly CinemaContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISeatService _seatService;
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly ILogger<TicketService> _logger;

        List<SeatDto> listOfSeatsreserved = new List<SeatDto>();

        public TicketService(CinemaContext dbContext,
                             IMapper mapper,
                             IShowtimesRepository showtimesRepository,
                             ISeatService seatService,
                             ILogger<TicketService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _seatService = seatService;
            _showtimesRepository = showtimesRepository;
            _logger = logger;
        }
           

        private void UpdateIsReserved()
        {

        }


        private async Task<TicketDto> CreateTicketDtoToReserveAsync(int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            Guid guid = Guid.NewGuid();

            // I grab the ShowTime:
            var showtime = await _showtimesRepository.GetWithSeatsByIdAsync(showtimeId, cancel);
            if (showtime == null)
            {
                _logger.LogError("no showtime found");

                return null;
            }

            ShowtimeDto showtimeDto = _mapper.Map<ShowtimeDto>(showtime);

            // get the auditoriumId:
            int auditoriumId = showtimeDto.AuditoriumId;

            // Find Seats Contiguous:
            var listSeatsToReserve = await FindSeatsContiguous(auditoriumId, nbrOfSeatsToReserve, showtimeDto);

            if (listSeatsToReserve == null || listOfSeatsreserved.Count == 0)
            {
                _logger.LogError("no seat to reserve");
                return null;
            }
                        

            TicketDto reservationDto = new()
            {
                Id = guid,
                ShowtimeId = showtimeId,
                Seats = listSeatsToReserve,
                CreatedTime = DateTime.Now,
                Paid = false,
                Showtime = showtimeDto

            };
            return reservationDto;


        }

        public async Task CreateTicketWithDelayAsync(int showtimeId, int nbrOfSeatsToReserve, CancellationToken cancel)
        {
            var reservationDto = await CreateTicketDtoToReserveAsync(showtimeId, nbrOfSeatsToReserve, cancel);


            if (reservationDto == null)
            {
                // logic
                _logger.LogError("");
                return;


            }


            // the logic to make seats reserved.
            // create a method to do that, because after i call this method to make it not reserved:
            // isReserved != is reserved

            // Start the delay task with the provided CancellationToken
            var delayTask = Task.Delay(TimeSpan.FromMinutes(10), cancel);

            // ContinueWith the cancellation logic
            await delayTask.ContinueWith(t =>
            {
                HandleCancellation(reservationDto);
            });


            return;
        }

        // To do: logic to cancel the ticket
        private void HandleCancellation(TicketDto reservationDto)
        {
            // i can make an enum: isReserved, isPaid, isFree then check

            // Check if seats are paid after 10 minutes
            if (!reservationDto.Paid)
            {
                ChangeTicketState(reservationDto);
                // call the method to make the seat not reserved.
                // If seats are not paid, cancel the reservation
                // Perform cancellation logic here
                // For example, set a reservation status flag to canceled
                _logger.LogInformation("Reservation {ReservationId} canceled because seats were not paid.", reservationDto.Id);
            }
        }

        private void ChangeTicketState(TicketDto ticketDto)
        {
            ticketDto.Paid = !ticketDto.Paid;
        }

        // recap this code, DRY
        private async Task<List<SeatDto>> FindSeatsContiguous(int auditoriumId, int nbrOfSeatsToReserve, ShowtimeDto showtimeDto)
        {
            return await Task.Run(() =>
            {
                List<SeatDto> listSeatReserved = new List<SeatDto>();

                int nbrOfSeatsPerRow;
                int nbrOfSeatsAvailable;
                int rowNbr;
                int seatNbr;
                int nbrOfSeatsContiguous = 0;
                int index = 0;
                int nbrOfRow;

                // get the list of seats :
                var seats = showtimeDto.Seats.ToList();

                var seat = seats.FirstOrDefault(s => s.IsReserved == false);

                if (seat == null)
                {
                    _logger.LogError("no more seats available");
                    return null;
                }

                listOfSeatsreserved.Add(seat);

                if (nbrOfSeatsToReserve == 1)
                {
                    seat.IsReserved = true;
                    return listOfSeatsreserved;
                }

                switch (auditoriumId)
                {
                    case 1:
                        nbrOfSeatsPerRow = 22;
                        nbrOfRow = 28;
                        break;
                    case 2:
                        nbrOfSeatsPerRow = 18;
                        nbrOfRow = 21;
                        break;
                    default:
                        nbrOfSeatsPerRow = 21;
                        nbrOfRow = 15;
                        break;
                }

                if (nbrOfSeatsToReserve > nbrOfSeatsPerRow)
                {
                    _logger.LogInformation("there are no enough seats contiguous");
                    return null;
                }


                rowNbr = seat.Row;
                seatNbr = seat.SeatNumber;

                nbrOfSeatsAvailable = nbrOfSeatsPerRow - seatNbr + 1;

                // grab the index of seat:
                index = seats.IndexOf(seat);


                for (int r = rowNbr; r <= nbrOfRow; r++)
                {

                    for (int s = seatNbr + 1; s <= nbrOfSeatsPerRow; s++)
                    {
                        index++;

                        if (seats[index].IsReserved == false)
                        {
                            nbrOfSeatsContiguous++;
                            listOfSeatsreserved.Add(seats[index]);

                            if (nbrOfSeatsContiguous == nbrOfSeatsToReserve)
                            {
                                // i should make them reserved, for that use for loop with index -- and nbrseatsToReserve
                                return listOfSeatsreserved;
                            }
                        }
                        else
                        {
                            nbrOfSeatsContiguous = 0;
                            listOfSeatsreserved.Clear();
                        }

                    }
                    listOfSeatsreserved.Clear();
                    nbrOfSeatsContiguous = 0;
                    seatNbr = 0;


                }

                return null;



                // check 10 min validation reservation
            });
        }
        
        

        public async Task ConfirmPayementAsync(TicketDto ticketDto, CancellationToken cancellation)
        {
            // check if the reservation is still alive

            ChangeTicketState(ticketDto);
            // call ConfirmPayementAsync from TicketRepo
            return;
        }
        
    }
}
