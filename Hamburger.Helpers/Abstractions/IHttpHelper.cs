using System.Threading.Tasks;

namespace Hamburger.Helpers.Abstractions
{
    public interface IHttpHelper
    {
        /// <summary>
        /// Set base URL for client.
        /// </summary>
        /// <param name="baseUrl">Base URL.</param>
        void SetBaseUrl(string baseUrl);

        /// <summary>
        /// Use JSON web token for authentication.
        /// </summary>
        /// <param name="bearerToken">Bear token.</param>
        void UseJwtAuthentication(string bearerToken);

        /// <summary>
        /// Use Api Key for authentication.
        /// </summary>
        /// <param name="key">Header key. Example: X-API-Key.</param>
        /// <param name="value">Header value.</param>
        void UseApiKeyAuthentication(string key, string value);

        /// <summary>
        /// Set timeout for client request.
        /// </summary>
        /// <param name="seconds">time in seconds.</param>
        void SetTimeout(int seconds);

        /// <summary>
        /// Send GET request.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="path">Request path.</param>
        /// <returns>Object type of T.</returns>
        Task<T> Get<T>(string path);

        /// <summary>
        /// Send POST request.
        /// </summary>
        /// <param name="path">Request path.</param>
        /// <param name="body">Request body.</param>
        Task Post(string path, object body);

        /// <summary>
        /// Send POST request.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="path">Request path.</param>
        /// <param name="body">Request body.</param>
        /// <returns>Object type of T.</returns>
        Task<T> Post<T>(string path, object body);

        /// <summary>
        /// Send PUT request.
        /// </summary>
        /// <param name="path">Request path.</param>
        /// <param name="body">Request body.</param>
        Task Put(string path, object body);

        /// <summary>
        /// Send PUT request.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="path">Request path.</param>
        /// <param name="body">Request body.</param>
        /// <returns>Object type of T.</returns>
        Task<T> Put<T>(string path, object body);

        /// <summary>
        /// Send DELETE request.
        /// </summary>
        /// <param name="path">Request path.</param>
        Task Delete(string path);

        /// <summary>
        /// Send DELETE request.
        /// </summary>
        /// <param name="path">Request path.</param>
        /// <returns>Object type of T.</returns>
        Task<T> Delete<T>(string path);
    }
}
