using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Types;
using System;
using System.Collections.Generic;

namespace PalringoApi.PluginManagement.Permissions
{
    /// <summary>
    /// The default instance of the IPermissionEngine
    /// </summary>
    public class PermissionEngine : IPermissionEngine
    {
        /// <summary>
        /// The permissions in order of their value
        /// </summary>
        public Permission[] PermissionOrder { get; set; } = Permission.Admin.AllFlags();

        /// <summary>
        /// Default instance of all of the permissions and how to check for them.
        /// </summary>
        public Dictionary<Permission, Func<User, Group, bool>> HasPermission { get; set; } = new Dictionary<Permission, Func<User, Group, bool>>
        {
            [Permission.All] = (u, g) => true,
            [Permission.ValidEmail] = (u, g) => u.ValidEmail,
            [Permission.Premium] = (u, g) => u.Premium,
            [Permission.AlphaTester] = (u, g) => u.AlphaTester,
            [Permission.Mod] = (u, g) => GroupMember(u, g) == Role.Mod,
            [Permission.Admin] = (u, g) => GroupMember(u, g) == Role.Admin,
            [Permission.Agent] = (u, g) => u.Agent,
            [Permission.Owner] = (u, g) => GroupMember(u, g) == Role.Owner,
            [Permission.Vip] = (u, g) => u.Vip,
            [Permission.Staff] = (u, g) => u.Staff,
            [Permission.SuperAdmin] = (u, g) => u.SuperAdmin,
            [Permission.AuthUser] = (u, g) => u.Id.IsAuthorized()
        };

        private static Role GroupMember(User user, Group group)
        {
            if (!group.Exists  || !group.Members.ContainsKey(user.Id))
                return Role.NotMember;

            return group.Members[user.Id].UserRole;
        }
    }
}
