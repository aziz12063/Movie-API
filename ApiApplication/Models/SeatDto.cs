

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
        public short SeatNumber { get; set; } = 0;
        public int AuditoriumId { get; set; }
        public bool IsReserved { get; set; }
    }
}
