using Hamburger.Models.Entities;
using Hamburger.Repository.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hamburger.Repository.EF.Repositories
{
    public class LoginSessionRepository : Repository<LoginSession>, ILoginSessionRepository
    {
        public LoginSessionRepository(ApplicationDbContext context) : base(context, nameof(LoginSession.Id))
        {
        }

        public async Task<IEnumerable<LoginSession>> GetUserLoginSessions(int userId)
        {
            var sessions = _context.LoginSessions.Where(x => x.UserId == userId);

            return await sessions.ToListAsync();
        }
    }
}
