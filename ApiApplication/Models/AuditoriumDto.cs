using ApiApplication.Database.Entities;
using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class AuditoriumDto
    {
        public int Id { get; set; }
        public List<ShowtimeDto> Showtimes { get; set; }
        public ICollection<SeatDto> Seats { get; set; }
    }
}
