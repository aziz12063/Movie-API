using ApiApplication.Database.Entities;
using System;
using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class ShowtimeDto
    {
        public int Id { get; set; }
        public MovieDto Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
        public ICollection<TicketDto> Tickets { get; set; }
        public ICollection<SeatDto> Seats { get; set; }


    }
}