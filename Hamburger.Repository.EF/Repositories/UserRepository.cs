using Hamburger.Helpers.Extensions;
using Hamburger.Models.Common;
using Hamburger.Models.Entities;
using Hamburger.Models.FilterModels;
using Hamburger.Repository.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hamburger.Repository.EF.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context, nameof(User.Id))
        {
        }

        public async Task<IEnumerable<User>> Get(UserFilterModel filterModel)
        {
            IQueryable<User> users = BuildFilterQuery(filterModel);

            return await users.ToListAsync();
        }

        private IQueryable<User> BuildFilterQuery(UserFilterModel filterModel)
        {
            IQueryable<User> users = BuildConditionalPhrase(filterModel);

            if (filterModel.Offset.HasValue && filterModel.Offset > -1)
            {
                users = users.Skip(filterModel.Offset.Value);
            }

            if (filterModel.Limit.HasValue && filterModel.Limit > -1)
            {
                users = users.Take(filterModel.Limit.Value);
            }

            return users;
        }

        public async Task<int> GetTotalCount(UserFilterModel filterModel)
        {
            IQueryable<User> users = BuildConditionalPhrase(filterModel);

            return await users.CountAsync();
        }

        private IQueryable<User> BuildConditionalPhrase(UserFilterModel filterModel)
        {
            IQueryable<User> users = _context.Users;

            if (!filterModel.Id.IsNullOrWhiteSpace())
            {
                users = users.Where(u => u.Id.ToString().Contains(filterModel.Id));
            }

            if (!filterModel.UserName.IsNullOrEmpty())
            {
                users = users.Where(u => u.UserName.Contains(filterModel.UserName));
            }

            if (!filterModel.Email.IsNullOrEmpty())
            {
                users = users.Where(u => u.Email.Contains(filterModel.Email));
            }

            if (filterModel.EmailConfirmed.HasValue)
            {
                users = users.Where(u => u.EmailConfirmed == filterModel.EmailConfirmed);
            }

            if (!filterModel.PhoneNumber.IsNullOrEmpty())
            {
                users = users.Where(u => u.PhoneNumber.Contains(filterModel.PhoneNumber));
            }

            if (filterModel.PhoneNumberConfirmed.HasValue)
            {
                users = users.Where(u => u.PhoneNumberConfirmed == filterModel.PhoneNumberConfirmed);
            }

            if (filterModel.TwoFactorEnabled.HasValue)
            {
                users = users.Where(u => u.TwoFactorEnabled == filterModel.TwoFactorEnabled);
            }

            if (filterModel.IsLockout.HasValue)
            {
                if (filterModel.IsLockout.Value)
                    users = users.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.Now);
                else users = users.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= DateTimeOffset.Now);
            }

            if (filterModel.LockoutEnabled.HasValue)
            {
                users = users.Where(u => u.LockoutEnabled == filterModel.LockoutEnabled);
            }

            if (filterModel.AccessFailedCount.HasValue)
            {
                switch (filterModel.AccessFailedCountOperator)
                {
                    case EnumComparisonOperator.LesserThan:
                        users = users.Where(u => u.AccessFailedCount < filterModel.AccessFailedCount);
                        break;
                    case EnumComparisonOperator.GreaterThan:
                        users = users.Where(u => u.AccessFailedCount > filterModel.AccessFailedCount);
                        break;
                    case EnumComparisonOperator.EqualTo:
                    default:
                        users = users.Where(u => u.AccessFailedCount == filterModel.AccessFailedCount);
                        break;
                }
            }

            if (!filterModel.Name.IsNullOrEmpty())
            {
                users = users.Where(u => u.FirstName.Contains(filterModel.Name) ||
                    u.MiddleName.Contains(filterModel.Name) ||
                    u.LastName.Contains(filterModel.Name));
            }

            if (filterModel.CreatedDate.HasValue)
            {
                users = users.Where(u => u.CreatedDate.Date == filterModel.CreatedDate.Value.Date);
            }

            if (filterModel.ModifiedDate.HasValue)
            {
                users = users.Where(u => u.ModifiedDate.Date == filterModel.ModifiedDate.Value.Date);
            }

            return users;
        }
    }
}
