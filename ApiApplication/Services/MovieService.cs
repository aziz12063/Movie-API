using ApiApplication.Cache;
using ApiApplication.Models;
using ApiApplication.ProvidedApi.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ApiApplication.Services
{
    public class MovieService : IMovieService
    {
        string apiUrl = "http://localhost:7172/v1/movies";
        string apiKey = "68e5fbda-9ec9-4858-97b2-4a8349764c63"; // i will perform this key
        int maxAttempt = 3;
        

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;
        private readonly IResponseCacheService _cacheService;

        
        public MovieService(HttpClient httpClient, 
                            IMapper mapper, 
                            ILogger<MovieService> logger,
                            IResponseCacheService cacheService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiUrl);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-apikey", apiKey);
            _mapper = mapper;
            _logger = logger;
            _cacheService = cacheService;
        }
   

        public async Task<MovieDto> GetMovieById(string id)
        {

            try
            {
                int attempt = 1;
                MovieDto movie = null;

                var cacheKey = $"movie-{id}";

                // i want to fetch just one movie
                while(attempt <= maxAttempt)
                {
                    Console.WriteLine("in GetMovieById, attempt N°: " + attempt);
                    var response = await _httpClient.GetAsync(apiUrl + "/" + id);
                    if (response.IsSuccessStatusCode)
                    {

                        MoviesApiEntity movieApi = null; // = new MoviesApiEntity();
                        string content = await response.Content.ReadAsStringAsync();

                        if (response.Content.Headers.ContentType.MediaType == "application/json")
                        {
                            movieApi = JsonSerializer.Deserialize<MoviesApiEntity>(content,
                            new JsonSerializerOptions()
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            });
                        }
                        else if (response.Content.Headers.ContentType.MediaType == "application/xml")
                        {
                            var serializer = new XmlSerializer(typeof(MoviesApiEntity));
                            // i use this:
                            //movieApi = (MoviesApiEntity)serializer.Deserialize(new StringReader(content));

                            // or this to destroy the StringReader object after geting the movieApi
                            using(var reader = new StreamReader(content))
                            {
                                movieApi = (MoviesApiEntity)serializer.Deserialize(reader);
                            }
                        }
                        
                        if(movieApi == null)
                        {
                            throw new Exception("Unsupported media type or deserialization error");
                        }


                         movie =  _mapper.Map<MovieDto>(movieApi);


                       
                        
                        
                        // i cache the movie response
                        await _cacheService.CacheResponseAsync(cacheKey, JsonSerializer.Serialize(movie));

                        // log the info of the movie to the console, not needed i can delete it later
                        Console.WriteLine("the Id is: " + movie.movieId);
                        Console.WriteLine("the Title is: " + movie.Title);
                        Console.WriteLine("the ImdbId is: " + movie.ImdbId);
                        Console.WriteLine("the ReleaseDate is: " + movie.ReleaseDate);

                        return movie;
                    }
                    attempt++;
                }

                // get the movie from the cach if  the API call failed
                var cacheContent = await _cacheService.GetCachedResponseAsync(cacheKey);
                if(!string.IsNullOrEmpty(cacheContent))
                {
                    movie = JsonSerializer.Deserialize<MovieDto>(cacheContent);
                }

                // i should costumize this exception
                if(movie == null)
                {
                    throw new Exception($"Movie with Id {id} could not be retrieved from the api or cache.");
                }
                
                 return movie;
            }

            catch (Exception ex)
            {
                throw new Exception("canot fetch data" + ex);
            }
        }


        // i can delete this

        public async Task<IEnumerable<MovieDto>> GetMovies()
        {
            try
            {

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var movies = new List<MovieDto>();
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.Content.Headers.ContentType.MediaType == "application/json")
                    {
                        movies = JsonSerializer.Deserialize<List<MovieDto>>(content,
                        new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                    }
                    else if (response.Content.Headers.ContentType.MediaType == "application/xml")
                    {
                        var serializer = new XmlSerializer(typeof(List<MovieDto>));
                        movies = (List<MovieDto>)serializer.Deserialize(new StringReader(content));
                    }

                    // i will process the movies list.
                    return movies;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // i need to handle that
                throw new Exception("canot fetch data" + ex);
            }



        }

       
    }
}
