using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Hamburger.Models.Entities
{
    public class Role : IdentityRole<int>
    {
        public Role() : base()
        { }
        public Role(string roleName) : base(roleName)
        { }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
