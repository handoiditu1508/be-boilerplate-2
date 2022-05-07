using Hamburger.Helpers;
using Hamburger.Models.Entities;
using Hamburger.Repository.Abstraction.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Repository.Dapper.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository() : base(AppConstants.Roles, nameof(Role.Id))
        {
        }

        public async Task<IEnumerable<Role>> GetUserRoles(int userId)
        {
            var param = new
            {
                RoleIdColumn = nameof(IdentityUserRole<int>.RoleId),
                UserRolesTable = AppConstants.UserRoles,
                UserIdColumn = nameof(IdentityUserRole<int>.UserId),
                UserIdValue = userId,
                RolesTable = _tableName,
                RolesTablePk = _primaryKey
            };

            string getRoleIdsSql = $"SELECT @RoleIdColumn FROM @UserRolesTable WHERE @UserIdColumn = @UserIdValue";
            string sql = $"SELECT * FROM @RolesTable WHERE @RolesTablePk IN ({getRoleIdsSql});";

            var result = await Get(sql, param);

            return result;
        }
    }
}
