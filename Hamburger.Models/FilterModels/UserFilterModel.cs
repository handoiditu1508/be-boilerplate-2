using Hamburger.Models.Common;
using System;

namespace Hamburger.Models.FilterModels
{
    public class UserFilterModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public bool? IsLockout { get; set; }
        public bool? LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public EnumComparisonOperator? AccessFailedCountOperator { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
