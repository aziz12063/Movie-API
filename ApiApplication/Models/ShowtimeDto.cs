using System;

namespace ApiApplication.Models
{
    public class ShowtimeDto
    {
        public int Id { get; set; }
        public MovieDto Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
        
        
    }
}
