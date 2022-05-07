using System.Collections.Generic;

namespace Hamburger.Helpers
{
    public static class PermissionClaimPolicies
    {
        public const string ViewUsers = "View Users";
        public const string UpdateUsers = "Update Users";
        public const string CreateUsers = "Create Users";
        public const string DeleteUsers = "Delete Users";

        public const string AdminViewUsers = "Admin View Users";
        public const string AdminUpdateUsers = "Admin Update Users";
        public const string AdminCreateUsers = "Admin Create Users";
        public const string AdminDeleteUsers = "Admin Delete Users";

        public const string ViewRoles = "View Roles";
        public const string UpdateRoles = "Update Roles";
        public const string CreateRoles = "Create Roles";
        public const string DeleteRoles = "Delete Roles";

        public const string AdminViewRoles = "Admin View Roles";
        public const string AdminUpdateRoles = "Admin Update Roles";
        public const string AdminCreateRoles = "Admin Create Roles";
        public const string AdminDeleteRoles = "Admin Delete Roles";

        private static readonly Dictionary<string, IEnumerable<string>> claimValues = new()
        {
            { ViewUsers, new string[] { PermissionClaimValues.ViewUsers } },
            { UpdateUsers, new string[] { PermissionClaimValues.UpdateUsers } },
            { CreateUsers, new string[] { PermissionClaimValues.CreateUsers } },
            { DeleteUsers, new string[] { PermissionClaimValues.DeleteUsers } },

            { AdminViewUsers, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.ViewUsers } },
            { AdminUpdateUsers, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.UpdateUsers } },
            { AdminCreateUsers, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.CreateUsers } },
            { AdminDeleteUsers, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.DeleteUsers } },

            { ViewRoles, new string[] { PermissionClaimValues.ViewRoles } },
            { UpdateRoles, new string[] { PermissionClaimValues.UpdateRoles } },
            { CreateRoles, new string[] { PermissionClaimValues.CreateRoles } },
            { DeleteRoles, new string[] { PermissionClaimValues.DeleteRoles } },

            { AdminViewRoles, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.ViewRoles } },
            { AdminUpdateRoles, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.UpdateRoles } },
            { AdminCreateRoles, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.CreateRoles } },
            { AdminDeleteRoles, new string[] { PermissionClaimValues.Administration, PermissionClaimValues.DeleteRoles } }
        };

        public static Dictionary<string, IEnumerable<string>> ClaimValues => claimValues;
    }

    public static class PermissionClaimValues
    {
        public const string Administration = "administration";

        public const string ViewUsers = "users.view";
        public const string UpdateUsers = "users.update";
        public const string CreateUsers = "users.create";
        public const string DeleteUsers = "users.delete";

        public const string ViewRoles = "roles.view";
        public const string UpdateRoles = "roles.update";
        public const string CreateRoles = "roles.create";
        public const string DeleteRoles = "roles.delete";
    }
}
