using ApiApplication.Cache;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Database.Repositories;
using ApiApplication.Services;
using ApiApplication.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplication.IntegrationTests
{
    public class DependenciesTestFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public DependenciesTestFixture()
        {
            var services = new ServiceCollection();

            // Register dependencies
            services.AddScoped<IShowtimesRepository, ShowtimesRepository>();
            services.AddScoped<ITicketsRepository, TicketsRepository>();
            services.AddScoped<IAuditoriumsRepository, AuditoriumsRepository>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IAuditoriumService, AuditoriumService>();
            //services.AddTransient<IReservationService, ReservationService>();
            services.AddScoped<IShowtimeService, ShowtimeService>(); //************
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();

            // Configure Redis distributed cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; // Redis connection string
                //options.InstanceName = "MovieCache"; // Cache instance name (optional), to search when i use it??
            });
            services.AddLogging();

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            // Clean up resources
            ServiceProvider?.Dispose();
        }
    }
}
