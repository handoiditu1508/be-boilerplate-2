namespace Hamburger.Models.Requests.UserService
{
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public string UserAgent { get; set; }
    }
}
