using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database;
using ApiApplication.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories;
using System.Net.Sockets;

namespace ApiApplication.Test.RepositoryTest
{
    public class AuditoriulRepositoryTest
    {
        private readonly DbContextOptions<CinemaContext> _dbContextOptions;
        

        public AuditoriulRepositoryTest()
        {    
           _dbContextOptions = new DbContextOptionsBuilder<CinemaContext>()
                                            .UseInMemoryDatabase(databaseName: "testDb")
                                            .Options;
            SeedData();
        }

        private void SeedData()
        {
            using (var context = new CinemaContext(_dbContextOptions))
            {
                context.Auditoriums.AddRange(
                    new AuditoriumEntity { auditoriumId = 1 },
                    new AuditoriumEntity { auditoriumId = 2 });
                context.SaveChanges();
            }
        }

        [Fact]
        public async void GetByIdWithSeatsAndShowtimesAsync_UnitTest()
        {
            int id = 1;
            CancellationToken cancellationToken = new CancellationToken();
            
            using(var context = new CinemaContext(_dbContextOptions))
            {
                var repo = new AuditoriumsRepository(context);

                // Act
                var result = await repo.GetByIdWithSeatsAndShowtimesAsync(id, cancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.auditoriumId);
            }
        }
    }
}
