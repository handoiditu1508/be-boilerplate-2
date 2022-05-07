using Hamburger.Helpers;
using Hamburger.Models.Entities;
using Hamburger.Repository.Abstraction.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Repository.Dapper.Repositories
{
    public class LoginSessionRepository : Repository<LoginSession>, ILoginSessionRepository
    {
        public LoginSessionRepository() : base(AppConstants.LoginSessions, nameof(LoginSession.Id))
        {
        }

        public async Task<IEnumerable<LoginSession>> GetUserLoginSessions(int userId)
        {
            var param = new
            {
                TableName = _tableName,
                UserIdCol = nameof(LoginSession.UserId),
                UserId = userId
            };

            var sql = $"SELECT * FROM @TableName where @UserIdCol = @UserId;";

            var result = await Get(sql, param);

            return result;
        }
    }
}
