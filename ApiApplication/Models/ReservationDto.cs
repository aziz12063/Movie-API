using System;
using System.Collections.Generic;

namespace ApiApplication.Models
{
    public class ReservationDto : TicketDto
    {

        public bool IsExpired { get; set; }

    }
}
