using ApiApplication.Models;
using System;
using System.Threading.Tasks;

namespace ApiApplication.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MovieDto> GetMovieById(string id);

    }
}
