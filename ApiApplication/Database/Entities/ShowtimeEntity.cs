using System;
using System.Collections.Generic;

namespace ApiApplication.Database.Entities
{
    public class ShowtimeEntity
    {
        public int showtimeId { get; set; }
        public MovieEntity Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
        public ICollection<TicketEntity> Tickets { get; set; }
        public AuditoriumEntity Auditorium { get; set; }
        
    }
}
