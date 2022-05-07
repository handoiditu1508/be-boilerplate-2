using Hamburger.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Repository.Abstraction.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>
        /// Get all roles of a user.
        /// </summary>
        /// <param name="userId">Id of the user to get roles.</param>
        /// <returns>List of roles.</returns>
        Task<IEnumerable<Role>> GetUserRoles(int userId);
    }
}
