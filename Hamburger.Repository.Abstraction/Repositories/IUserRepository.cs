using Hamburger.Models.Entities;
using Hamburger.Models.FilterModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Repository.Abstraction.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Find and filter users.
        /// </summary>
        /// <param name="filterModel">Filter criteria.</param>
        /// <returns>List of users.</returns>
        Task<IEnumerable<User>> Get(UserFilterModel filterModel);

        /// <summary>
        /// Count total records.
        /// </summary>
        /// <param name="filterModel">Filter criteria.</param>
        /// <returns>Total records after filter.</returns>
        Task<int> GetTotalCount(UserFilterModel filterModel);

        /// <summary>
        /// Get user include user's roles.
        /// </summary>
        /// <param name="id">User's primary key.</param>
        /// <returns>User with user's roles included.</returns>
        Task<User> GetWithRoles(int id);
    }
}
