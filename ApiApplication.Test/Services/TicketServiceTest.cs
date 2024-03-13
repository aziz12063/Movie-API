using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiApplication.Models;
using ApiApplication.Services;

namespace ApiApplication.Test.Services
{
    public class TicketServiceTest
    {
        //[Fact]
        //public void TheCollectionOfTickerAreGenerated()
        //{
        //    int nbrOfTicket = 3;
        //    List<Guid> guid = new List<Guid>();
        //    guid.Add(new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA"));
        //    guid.Add(new Guid("6B30FC40-CA47-1067-B31D-00DD010662DB"));
        //    guid.Add(new Guid("6B30FC40-CA47-1067-B31D-00DD010662DC"));


        //    TicketService ticketService = new TicketService();

        //    List<TicketDto> tickets = ticketService.GenerateTicketCollection(guid);

        //    TicketDto ticketTest = new TicketDto
        //    {
        //        Id = Guid.NewGuid(),
        //        ShowtimeId = 1,
        //        CreatedTime = DateTime.Now
        //    };

        //    // verify the list of Ticket
        //    Assert.NotNull(tickets);

        //    // the nbrOfTicket will be the nbr of seat in an auditorium
        //    Assert.Equal(nbrOfTicket, tickets.Count);

        //    // verify each Ticket
        //    TicketDto ticket = tickets[0];
        //    TicketDto ticket1 = tickets[1];
        //    TicketDto ticket2 = tickets[2];

        //    Assert.NotNull(ticket);
        //    Assert.NotNull(ticket1);
        //    Assert.NotNull(ticket2);


        //    // verify the members of Ticket:
        //    Assert.Equal(guid[0], ticket.Id);
        //    Assert.Equal(guid[1], ticket1.Id);

        //    // verify the showtime

        //    Assert.Equal(1, ticket1.ShowtimeId);
        //    Assert.Equal(2, ticket2.ShowtimeId);
            

        //    // modify the time to now
        //    Assert.Equal(ticket1.CreatedTime, new DateTime(2024, 03, 05, 14, 00, 00));
        //    Assert.Equal(ticket2.CreatedTime, new DateTime(2024, 03, 05, 16, 00, 00));
           
            
           

        //}
    }
}
