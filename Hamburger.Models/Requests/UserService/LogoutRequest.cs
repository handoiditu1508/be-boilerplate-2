namespace Hamburger.Models.Requests.UserService
{
    public class LogoutRequest
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
