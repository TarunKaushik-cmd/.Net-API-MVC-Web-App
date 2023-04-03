using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EmployeeWebApp.Services
{
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> AuthenticateAsync(string username, string password)
        {
            // get API endpoint and credentials from configuration
            var apiEndpoint = _configuration["Authentication:ApiEndpoint"];

            // build request to authenticate user
            var request = new HttpRequestMessage(HttpMethod.Post, $"{apiEndpoint}login");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "username", username },
            { "password", password },
        });

            // send request to API
            var response = await _httpClient.SendAsync(request);

            return response;
        }

        internal async Task<HttpResponseMessage> CreateUserAsync(Dictionary<string, string> userData)
        {
            var apiEndpoint = _configuration["Authentication:ApiEndpoint"];

            // build request to create user
            var request = new HttpRequestMessage(HttpMethod.Post, $"{apiEndpoint}/api/users");
            request.Content = new FormUrlEncodedContent(userData);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

            // send request to API
            var response = await _httpClient.SendAsync(request);

            return response;
        }
        public async Task<string> GetAccessTokenAsync()
        {
            // get API endpoint and credentials from configuration
            var tokenEndpoint = _configuration["Authentication:TokenEndpoint"];
            var clientId = _configuration["Authentication:ClientId"];
            var clientSecret = _configuration["Authentication:ClientSecret"];

            // build request to get access token
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
            });

            // send request to API
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // parse access token from response
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JObject.Parse(responseContent)["access_token"].ToString();

                return token;
            }
            else
            {
                // handle error
                throw new Exception($"Failed to get access token: {response.ReasonPhrase}");
            }
        }

    }
}
