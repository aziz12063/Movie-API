﻿using System.Collections.Generic;

namespace ApiApplication.Database.Entities
{
    public class AuditoriumEntity
    {
        public int auditoriumId { get; set; }
        public List<ShowtimeEntity> Showtimes { get; set; }
        public ICollection<SeatEntity> Seats { get; set; }
       
    }
}
