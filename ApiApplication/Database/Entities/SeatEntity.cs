﻿

namespace ApiApplication.Database.Entities
{
    public class SeatEntity
    {
        public int seatId { get; set; }
        public short Row { get; set; }
        public short SeatNumber { get; set; }
        public int AuditoriumId { get; set; }
        public AuditoriumEntity Auditorium { get; set; }
        public bool IsReserved { get; set; }
    }
}
