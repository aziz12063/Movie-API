﻿using ApiApplication.Database;
using ApiApplication.Database.Entities;
using ApiApplication.Models;
using ApiApplication.ProvidedApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        
        public MovieService(HttpClient httpClient, IMapper mapper, ILogger<MovieService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(apiUrl);
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-apikey", apiKey);
            _mapper = mapper;
            _logger = logger;
        }
   

        // i will implement this method when i work with Provided-api
        public async Task<MovieDto> GetMovieById(string id)
        {

            try
            {
                int attempt = 1;
                // i want to fetch just one movie
                while(attempt <= maxAttempt)
                {
                    Console.WriteLine("in GetMovieById, attempt N°: " + attempt);
                    var response = await _httpClient.GetAsync(apiUrl + "/" + id);
                    if (response.IsSuccessStatusCode)
                    {

                        var movieApi = new MoviesApiEntity();
                        var content = await response.Content.ReadAsStringAsync();
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
                            movieApi = (MoviesApiEntity)serializer.Deserialize(new StringReader(content));
                        }
                        
                        var movie =  _mapper.Map<MovieDto>(movieApi);

                        Console.WriteLine("the Id is: " + movie.Id);
                        Console.WriteLine("the Title is: " + movie.Title);
                        Console.WriteLine("the ImdbId is: " + movie.ImdbId);
                        Console.WriteLine("the ReleaseDate is: " + movie.ReleaseDate);

                        return movie;
                    }
                    attempt++;
                }
                
                 return null;
            }
            catch (Exception ex)
            {
                throw new Exception("canot fetch data" + ex);
            }
        }


        

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