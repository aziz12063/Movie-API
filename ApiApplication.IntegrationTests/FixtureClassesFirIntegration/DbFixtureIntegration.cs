using ApiApplication.Database.Entities;
using ApiApplication.Database;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Serilog;


namespace ApiApplication.IntegrationTests.FixtureClassesFirIntegration
{
    public class DbFixtureIntegration : IDisposable
    {
        private DbContextOptions<CinemaContext> _dbContextOptions { get; }
        public CancellationToken cancellationToken { get; private set; }
        public CinemaContext context;
        public  IMapper _mapper;
        public Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;


        public DbFixtureIntegration()
        {
            _dbContextOptions = new DbContextOptionsBuilder<CinemaContext>()
                                            .UseInMemoryDatabase(databaseName: "testDb2")
                                            .Options;
            context = new CinemaContext(_dbContextOptions);
            var config = new MapperConfiguration(cfg =>
            {
                // Automatically load profiles from the assembly containing the profiles
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });
            _mapper = config.CreateMapper();
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });

            SeedData();
            cancellationToken = new CancellationToken();


        }

        public void ResetDatabase()
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            SeedData();
        }

        private void SeedData()
        {
            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            
            
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

            AuditoriumEntity Auditorium = new AuditoriumEntity
            {
                AuditoriumId = 10,
                Seats = new List<SeatEntity>
                {
                    new SeatEntity { Row = 1, SeatNumber = 2, IsReserved = false },
                    new SeatEntity { Row = 1, SeatNumber = 3, IsReserved = false },
                    new SeatEntity { Row = 1, SeatNumber = 4, IsReserved = false }
                }
            };

            context.Auditoriums.Add(Auditorium);

            ShowtimeEntity showtimeEntity = new ShowtimeEntity
            {
                ShowtimeId = 10,
                Auditorium = Auditorium
            };

            context.Showtimes.Add(showtimeEntity);  

            context.SaveChanges();
            
        }


        public void Dispose()
        {
            context.Dispose();
        }
    }
}
