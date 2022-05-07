using Hamburger.Models.Entities;
using Hamburger.Repository.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hamburger.Repository.EF.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context, nameof(Role.Id))
        {
        }

        public async Task<IEnumerable<Role>> GetUserRoles(int userId)
        {
            var userRoles = _context.UserRoles.Where(x => x.UserId == userId);

            var roles = _context.Roles.Where(r => userRoles.Select(ur => ur.RoleId).Contains(r.Id));

            return await roles.ToListAsync();
        }
    }
}
