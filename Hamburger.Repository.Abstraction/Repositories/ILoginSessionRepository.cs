using Hamburger.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Repository.Abstraction.Repositories
{
    public interface ILoginSessionRepository : IRepository<LoginSession>
    {
        Task<IEnumerable<LoginSession>> GetUserLoginSessions(int userId);
    }
}
