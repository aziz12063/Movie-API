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
using ApiApplication.Test.TestFixture;
using Microsoft.Extensions.Options;

namespace ApiApplication.Test.RepositoryTest
{
    public class AuditoriulRepositoryTest : IClassFixture<DataBaseFixture>
    {
        private readonly DataBaseFixture _fixture;

        public AuditoriulRepositoryTest(DataBaseFixture fixture)
        {
            _fixture = fixture;
        }

       
        
        [Fact]
        public async void GetByIdWithSeatsAndShowtimesAsync_AuditNotExist_ShouldReturnNull()
        {
            int id = 3;
            
            using(var context = new CinemaContext(_fixture._dbContextOptions))
            {
                var repo = new AuditoriumsRepository(context);

                // Act
                var result = await repo.GetByIdWithSeatsAndShowtimesAsync(id, _fixture.cancellationToken);

                // Assert
                Assert.Null(result);
                                
            }
        }

        [Theory]
        //[InlineData(id, expectedAuditoriumId)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public async void GetByIdWithSeatsAndShowtimesAsync_DataDrivenTest(int id, int expectedAuditoriumId)
        {
            using (var context = new CinemaContext(_fixture._dbContextOptions))
            {
                var repo = new AuditoriumsRepository(context);

                // Act
                var result = await repo.GetByIdWithSeatsAndShowtimesAsync(id, _fixture.cancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expectedAuditoriumId, result.auditoriumId);
            }
        }

        


    }
}
