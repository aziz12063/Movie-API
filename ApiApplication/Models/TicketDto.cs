using System;
using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class TicketDto
    {
        public TicketDto()
        {
            CreatedTime = DateTime.Now;
            Paid = false;
        }

        public Guid Id { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime CreatedTime { get; set; }
        public ICollection<SeatDto> Seats { get; set; }
        public bool Paid { get; set; } 
        public ShowtimeDto Showtime { get; set; }
    }
}
