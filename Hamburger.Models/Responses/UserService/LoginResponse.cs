using Hamburger.Models.UserService;
using System;

namespace Hamburger.Models.Responses.UserService
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public UserFullDetails User { get; set; }
    }
}
