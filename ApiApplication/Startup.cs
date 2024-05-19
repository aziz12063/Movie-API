using ApiApplication.Database;
using ApiApplication.Database.Repositories;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services;
using ApiApplication.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;


namespace ApiApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register my services
            services.AddTransient<IShowtimesRepository, ShowtimesRepository>();
            services.AddTransient<ITicketsRepository, TicketsRepository>();
            services.AddTransient<IAuditoriumsRepository, AuditoriumsRepository>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IAuditoriumService, AuditoriumService>();
            //services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IShowtimeService, ShowtimeService>(); //************
            services.AddTransient<ITicketService, TicketService>();
            services.AddTransient<ISeatService, SeatService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Configure Redis distributed cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; // Redis connection string
                options.InstanceName = "MovieCache"; // Cache instance name (optional)
            });

            // Configure DbContext
            services.AddDbContext<CinemaContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            // Add controllers, API explorer, and Swagger
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add HttpClient
            services.AddHttpClient();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // configure logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .CreateLogger();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
                
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SampleData.Initialize(app);
        }      
    }
}
