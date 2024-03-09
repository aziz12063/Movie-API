using ApiApplication.Models;
using System;
using System.Threading.Tasks;

namespace ApiApplication
{
    public interface IMovieService
    {
        Task<MovieDto> GetMovieById(string id);

    }
}
