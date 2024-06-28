using ApiApplication.Database.Entities;
using ApiApplication.Database;
using Microsoft.EntityFrameworkCore;


namespace SharedFixtureTest
{
    public class DbFixtureShared : IDisposable
    {
        public DbContextOptions<CinemaContext> _dbContextOptions { get; private set; }
        public CancellationToken cancellationToken { get; private set; }
        public CinemaContext context;

        public DbFixtureShared()
        {
            _dbContextOptions = new DbContextOptionsBuilder<CinemaContext>()
                                            .UseInMemoryDatabase(databaseName: "testDb2")
                                            .Options;
            context = new CinemaContext(_dbContextOptions);

            SeedData();
            cancellationToken = new CancellationToken();

        }

        private void SeedData()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            
            
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
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
