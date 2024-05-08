using ApiApplication.Database.Entities;
using ApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication
{
    public interface IShowtimeService
    {
        Task<bool> CreateShowTime(ShowtimeDto showtimeDto, CancellationToken cancel);
        //Task<bool> CreateShowTime(string movieId, int auditoriumId, DateTime sessionDate, CancellationToken cancel);
    }
}
