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
using ApiApplication.Cache;
using ApiApplication.TimeConstraint;
using ApiApplication.Middleware;
using System.Text.Json.Serialization;


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
            
            // Configure Redis distributed cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; // Redis connection string
                //options.InstanceName = "MovieCache"; // Cache instance name (optional), to search when i use it??
            });

            // Configure DbContext
            services.AddDbContext<CinemaContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            }, ServiceLifetime.Scoped);

            services.AddRouting(option =>
            {
                option.ConstraintMap.Add("CustomDate", typeof(CustomDateTimeConstraint));
            });

            // Add controllers, API explorer, and Swagger
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            // i configure swagger to display the URL with endpoint
            /*
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Cineme API",
                    Version = "v1"
                });
            });*/

            // Add HttpClient
            services.AddHttpClient();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // configure logging
            // the logging appeare twice, maybe i should this delete configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .CreateLogger();
            

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                // i configure swagger to display the URL with endpoint
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cinema API v1"));
                app.UseDeveloperExceptionPage();
                
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RequestTimeMiddleware>();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SampleData.Initialize(app);
            
           

        }
        
    }
}
