using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class AuditoriumDto
    {
        public int AuditoriumId { get; set; }
        public List<ShowtimeDto> Showtimes { get; set; }
        public ICollection<SeatDto> Seats { get; set; }
    }
}
