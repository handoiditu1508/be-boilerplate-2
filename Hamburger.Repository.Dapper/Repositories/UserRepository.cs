using Dapper;
using Hamburger.Helpers;
using Hamburger.Helpers.Extensions;
using Hamburger.Models.Common;
using Hamburger.Models.Entities;
using Hamburger.Models.FilterModels;
using Hamburger.Repository.Abstraction.Repositories;
using Hamburger.Repository.Dapper.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hamburger.Repository.Dapper.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository() : base(AppConstants.Users, nameof(User.Id))
        {
        }

        public async Task<IEnumerable<User>> Get(UserFilterModel filterModel)
        {
            var sql = BuildFilterQuery(filterModel);

            var result = await Get(sql);

            return result;
        }

        public async Task<int> GetTotalCount(UserFilterModel filterModel)
        {
            var sql = BuildTotalCountFilterQuery(filterModel);

            var result = await ExecuteScalar<int>(sql);

            return result;
        }

        private string BuildFilterQuery(UserFilterModel filterModel)
        {
            string sql = $"SELECT * FROM {_tableName} WHERE 1";

            sql += BuildConditionalPhrase(filterModel);

            if (filterModel.Offset.HasValue && filterModel.Offset > -1)
            {
                sql += $" OFFSET {filterModel.Offset} ROWS";
            }
            else sql += " OFFSET 0 ROWS";

            if (filterModel.Limit.HasValue && filterModel.Limit > 0)
            {
                sql += $" FETCH NEXT {filterModel.Limit} ROWS ONLY";
            }

            sql += ";";

            return sql;
        }

        private string BuildTotalCountFilterQuery(UserFilterModel filterModel)
        {
            string sql = $"SELECT COUNT(*) FROM {_tableName} WHERE 1";

            sql += BuildConditionalPhrase(filterModel);

            sql += ";";

            return sql;
        }

        private string BuildConditionalPhrase(UserFilterModel filterModel)
        {
            string sql = string.Empty;

            if (!filterModel.Id.IsNullOrWhiteSpace())
            {
                sql += $" AND {nameof(User.Id)} LIKE '%{filterModel.Id}%'";
            }

            if (!filterModel.UserName.IsNullOrEmpty())
            {
                sql += $" AND {nameof(User.UserName)} LIKE '%{filterModel.UserName}%'";
            }

            if (!filterModel.Email.IsNullOrEmpty())
            {
                sql += $" AND {nameof(User.Email)} LIKE '%{filterModel.Email}%'";
            }

            if (filterModel.EmailConfirmed.HasValue)
            {
                sql += $" AND {nameof(User.EmailConfirmed)} = {Convert.ToInt16(filterModel.EmailConfirmed)}";
            }

            if (!filterModel.PhoneNumber.IsNullOrEmpty())
            {
                sql += $" AND {nameof(User.PhoneNumber)} LIKE '%{filterModel.PhoneNumber}%'";
            }

            if (filterModel.PhoneNumberConfirmed.HasValue)
            {
                sql += $" AND {nameof(User.PhoneNumberConfirmed)} = {Convert.ToInt16(filterModel.PhoneNumberConfirmed)}";
            }

            if (filterModel.TwoFactorEnabled.HasValue)
            {
                sql += $" AND {nameof(User.TwoFactorEnabled)} = {Convert.ToInt16(filterModel.TwoFactorEnabled)}";
            }

            if (filterModel.IsLockout.HasValue)
            {
                if (filterModel.IsLockout.Value)
                    sql += $" AND {nameof(User.LockoutEnd)} IS NOT NULL AND {nameof(User.LockoutEnd)} > SYSDATETIMEOFFSET()";
                else sql += $" AND ({nameof(User.LockoutEnd)} IS NULL OR {nameof(User.LockoutEnd)} <= SYSDATETIMEOFFSET())";
            }

            if (filterModel.LockoutEnabled.HasValue)
            {
                sql += $" AND {nameof(User.LockoutEnabled)} = {Convert.ToInt16(filterModel.LockoutEnabled)}";
            }

            if (filterModel.AccessFailedCount.HasValue)
            {
                switch (filterModel.AccessFailedCountOperator)
                {
                    case EnumComparisonOperator.LesserThan:
                        sql += $" AND {nameof(User.AccessFailedCount)} < {filterModel.AccessFailedCount}";
                        break;
                    case EnumComparisonOperator.GreaterThan:
                        sql += $" AND {nameof(User.AccessFailedCount)} > {filterModel.AccessFailedCount}";
                        break;
                    case EnumComparisonOperator.EqualTo:
                    default:
                        sql += $" AND {nameof(User.AccessFailedCount)} = {filterModel.AccessFailedCount}";
                        break;
                }
            }

            if (!filterModel.Name.IsNullOrEmpty())
            {
                sql += $" AND ({nameof(User.FirstName)} LIKE '%{filterModel.Name}%' OR {nameof(User.MiddleName)} LIKE '%{filterModel.Name}%' OR {nameof(User.LastName)} LIKE '%{filterModel.Name}%')";
            }

            if (filterModel.CreatedDate.HasValue)
            {
                sql += $" AND {DynamicQuery.GetDateFromDateTimeFunction(nameof(User.CreatedDate))} = {DynamicQuery.GetDateFromDateTimeFunction("GETUTCDATE()")}";
            }

            if (filterModel.ModifiedDate.HasValue)
            {
                sql += $" AND {DynamicQuery.GetDateFromDateTimeFunction(nameof(User.ModifiedDate))} = {DynamicQuery.GetDateFromDateTimeFunction("GETUTCDATE()")}";
            }

            return sql;
        }


        public async Task<User> GetFullDetails(int id)
        {
            // reference: https://www.learndapper.com/relationships

            var param = new
            {
                Id = id
            };

            var sql = $@"SELECT u.*, r.*
                        FROM {_tableName} u
                        INNER JOIN {AppConstants.UserRoles} ur ON ur.{nameof(IdentityUserRole<int>.UserId)} = u.{_primaryKey}
                        INNER JOIN {AppConstants.Roles} r ON r.{nameof(Role.Id)} = ur.{nameof(IdentityUserRole<int>.RoleId)}
                        WHERE u.{_primaryKey} = @Id;";

            IEnumerable<User> users;

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                users = await connection.QueryAsync<User, Role, User>(sql, (user, role) =>
                {
                    user.Roles.Add(role);
                    return user;
                }, splitOn: nameof(Role.Id), param: param);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            var result = users.GroupBy(u => u.Id).Select(g =>
            {
                var groupedUser = g.First();
                groupedUser.Roles = g.Select(u => u.Roles.Single()).ToList();
                return groupedUser;
            });

            return result.Single();
        }
    }
}
