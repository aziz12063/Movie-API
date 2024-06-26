using ApiApplication.Database;
using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplication.Test.TestFixture
{
    public class DataBaseFixture : IDisposable
    {
        public DbContextOptions<CinemaContext> _dbContextOptions {get; private set;}
        public CancellationToken cancellationToken { get; private set; }

        public DataBaseFixture()
        {
            _dbContextOptions = new DbContextOptionsBuilder<CinemaContext>()
                                            .UseInMemoryDatabase(databaseName: "testDb")
                                            .Options;
            SeedData();
            cancellationToken = new CancellationToken();

        }

        private void SeedData()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            using (var context = new CinemaContext(_dbContextOptions))
            {
                context.Auditoriums.AddRange(
                    new AuditoriumEntity { auditoriumId = 1 },
                    new AuditoriumEntity { auditoriumId = 2 });

                context.Showtimes.AddRange(
                    new ShowtimeEntity { showtimeId = 1 },
                    new ShowtimeEntity { showtimeId = 2 }
                    );

                context.Tickets.AddRange(
                    new TicketEntity { TicketId = guid1 },
                    new TicketEntity { TicketId = guid2 }
                    );
                context.SaveChanges();
            }
        }
        public void Dispose()
        {
           
        }
    }
}
