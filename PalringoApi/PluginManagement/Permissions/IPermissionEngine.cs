using System;
using System.Collections.Generic;
using PalringoApi.Subprofile;

namespace PalringoApi.PluginManagement.Permissions
{
    /// <summary>
    /// The interface for permissions checking
    /// </summary>
    public interface IPermissionEngine
    {
        /// <summary>
        /// How to check for certain permissions
        /// </summary>
        Dictionary<Permission, Func<User, Group, bool>> HasPermission { get; set; }
        /// <summary>
        /// The order of permissions (by index)
        /// </summary>
        Permission[] PermissionOrder { get; set; }
    }
}