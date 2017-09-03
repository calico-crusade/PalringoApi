namespace PalringoApi
{
    /// <summary>
    /// An enum that dictates the different permission levels.
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// Anyone can use the plugin
        /// </summary>
        All = 0,
        /// <summary>
        /// Anyone with a valid email can use the plugin
        /// </summary>
        ValidEmail = 1,
        /// <summary>
        /// Anyone who is premium can use the plugin
        /// </summary>
        Premium = 2,
        /// <summary>
        /// Anyone who is an Alpha Tester can use the plugin
        /// </summary>
        AlphaTester = 3,
        /// <summary>
        /// Anyone with mod power in the group can use the plugin
        /// </summary>
        Mod = 4,
        /// <summary>
        /// Anyone who has admin power in the group can use the plugin
        /// </summary>
        Admin = 5,
        /// <summary>
        /// Anyone who has the agent tag can use the plugin
        /// </summary>
        Agent = 6,
        /// <summary>
        /// Anyone who has owner abilities can use the plugin
        /// </summary>
        Owner = 7,
        /// <summary>
        /// Anyone who has VIP abilities can use the plugin
        /// </summary>
        Vip = 8,
        /// <summary>
        /// Anyone who has staff abilities can use the plugin
        /// </summary>
        Staff = 9,
        /// <summary>
        /// Anyone with super admin abilities can use the plugin
        /// </summary>
        SuperAdmin = 10,
        /// <summary>
        /// Anyone who is marked as an Authorized user can use the Plugin these are set using <see cref="Plugins.Permissions.AuthorizationEngine"/>
        /// </summary>
        AuthUser = 11
    }
}
