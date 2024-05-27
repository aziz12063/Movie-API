using ApiApplication.Database.Entities;
using ApiApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication
{
    public interface IShowtimeService
    {
        //Task<bool> CreateShowTime(ShowtimeDto showtimeDto, CancellationToken cancel);
        Task<ShowtimeDto> CreateShowTime(ShowtimeDto showtimeDto, CancellationToken cancel);
        Task<ShowtimeDto> GetShowtimeByAuditoriumIdAndSessionDate(int auditoriumId, DateTime sessionDate, CancellationToken cancellationToken);

        Task<ShowtimeDto> GetShowtimeWithMovieById(int Id, CancellationToken cancellation);
        Task<bool> ShowtimeExistAsync(int auditoriumId, DateTime sessionDate);


    }
}
