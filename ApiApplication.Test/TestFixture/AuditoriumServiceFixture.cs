using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database;
using ApiApplication.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiApplication.Test.TestFixture
{
    public class AuditoriumServiceFixture : IDisposable
    {
        // I mock the IAuditoriumsRepository interface
        public readonly Mock<IAuditoriumsRepository> _mockAuditoriumRepository;
        public readonly AuditoriumService _AuditoriumService;
        //public readonly Mock<IMapper> _mockMapper;
        //public readonly CinemaContext _inMemoryCinemaContext;

        public AuditoriumServiceFixture()
        {
            _mockAuditoriumRepository = new Mock<IAuditoriumsRepository>();
           // _mockMapper = new Mock<IMapper>();
            // configure an in-memory Db context
           // var option = new DbContextOptionsBuilder<CinemaContext>().UseInMemoryDatabase(databaseName: "TestDB").Options;
            //_inMemoryCinemaContext = new CinemaContext(option);

            _AuditoriumService = new AuditoriumService(_mockAuditoriumRepository.Object);
        }

        public void Dispose()
        {
           // _inMemoryCinemaContext?.Dispose();
        }
    }
}
