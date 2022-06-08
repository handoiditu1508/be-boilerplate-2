using System.Collections.Generic;

namespace Hamburger.Models.UserService
{
    public class UserFullDetails : UserViewModel
    {
        public IEnumerable<string> Roles { get; set; }
    }
}
