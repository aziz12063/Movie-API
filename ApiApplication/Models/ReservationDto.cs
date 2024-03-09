using System;
using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public List<List<int>> nbrOfSeatsreserved = new List<List<int>>();
        // or
        //public int SeatNbr { get; set; }
        public int AuditoriumId { get; set; }
        public string MovieName { get; set; }
        public DateTime ReservationTime { get; set; }
        public bool IsExpired { get; set; }
        public bool IsPaid { get; set; }

        

    }
}
