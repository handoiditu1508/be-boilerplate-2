using System.Collections.Generic;

namespace Hamburger.Models.Requests.UserService
{
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public string UserAgent { get; set; }
    }
}
