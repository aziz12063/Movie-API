

using ApiApplication.Database.Entities;
using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class SeatDto
    {
        public SeatDto()
        {
            IsReserved = false;
        }

        public int seatId { get; set; }
        public short Row { get; set; }
        public short SeatNumber { get; set; }
        public int AuditoriumId { get; set; }
        public bool IsReserved { get; set; }
        //public ICollection<TicketDto> Tickets { get; set; } // 
        public AuditoriumEntity Auditorium { get; set; }
    }
}
