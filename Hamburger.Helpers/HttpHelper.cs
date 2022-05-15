using Hamburger.Helpers.Abstractions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hamburger.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        private readonly HttpClient _httpClient;

        public HttpHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(120);
        }

        public void SetBaseUrl(string baseUrl)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public void UseJwtAuthentication(string bearerToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        public void UseApiKeyAuthentication(string key, string value)
        {
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key, value);
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }

        public void SetTimeout(int seconds)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(seconds);
        }

        public async Task<T> Get<T>(string path)
        {
            var response = await _httpClient.GetAsync(path);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        public async Task Post(string path, object body)
        {
            var response = await _httpClient.PostAsJsonAsync(path, body);

            response.EnsureSuccessStatusCode();
        }

        public async Task<T> Post<T>(string path, object body)
        {
            var response = await _httpClient.PostAsJsonAsync(path, body);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        public async Task Put(string path, object body)
        {
            var response = await _httpClient.PutAsJsonAsync(path, body);

            response.EnsureSuccessStatusCode();
        }

        public async Task<T> Put<T>(string path, object body)
        {
            var response = await _httpClient.PutAsJsonAsync(path, body);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        public async Task Delete(string path)
        {
            var response = await _httpClient.DeleteAsync(path);

            response.EnsureSuccessStatusCode();
        }

        public async Task<T> Delete<T>(string path)
        {
            var response = await _httpClient.DeleteAsync(path);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<T>();

            return result;
        }
    }
}
