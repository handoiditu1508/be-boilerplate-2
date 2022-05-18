namespace Hamburger.Models.Requests.UserService
{
    public class LogoutRequest
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
    }
}
