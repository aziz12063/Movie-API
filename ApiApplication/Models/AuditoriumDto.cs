using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class AuditoriumDto
    {
        public int auditoriumId { get; set; }
        public List<ShowtimeDto> Showtimes { get; set; }
        public ICollection<SeatDto> Seats { get; set; }
    }
}
