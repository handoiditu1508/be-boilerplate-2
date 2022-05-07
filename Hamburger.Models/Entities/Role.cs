using Microsoft.AspNetCore.Identity;

namespace Hamburger.Models.Entities
{
    public class Role : IdentityRole<int>
    {
        public Role() : base()
        { }
        public Role(string roleName) : base(roleName)
        { }
    }
}
