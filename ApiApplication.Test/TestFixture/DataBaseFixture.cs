using ApiApplication.Database;
using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;


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
                    new AuditoriumEntity { AuditoriumId = 1 },
                    new AuditoriumEntity { AuditoriumId = 2 });

                context.Showtimes.AddRange(
                    new ShowtimeEntity { ShowtimeId = 1 },
                    new ShowtimeEntity { ShowtimeId = 2 }
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
