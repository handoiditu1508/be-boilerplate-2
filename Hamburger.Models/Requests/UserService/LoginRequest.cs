namespace Hamburger.Models.Requests.UserService
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string UserAgent { get; set; }
    }
}
