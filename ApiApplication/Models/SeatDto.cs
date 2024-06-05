using ApiApplication.Database.Entities;


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
        
        public AuditoriumEntity Auditorium { get; set; }
    }
}
