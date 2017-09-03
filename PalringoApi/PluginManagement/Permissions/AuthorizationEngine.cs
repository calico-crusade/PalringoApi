using PalringoApi.Networking;
using PalringoApi.Subprofile;
using System.Collections.Generic;
using System.Linq;

namespace PalringoApi.PluginManagement.Permissions
{
    /// <summary>
    /// The engine used to identify Authorized Users
    /// </summary>
    public static class AuthorizationEngine
    {
        private static List<UserId> Changeable = new List<UserId>();
        private static UserId[] SuperUsers = new UserId[] { 39833624, 43681734 };

        /// <summary>
        /// The collection of Authorized Users
        /// </summary>
        public static UserId[] AuthorizedUsers => Changeable.Union(SuperUsers).ToArray();

        /// <summary>
        /// Checks whether a user is authorized or not.
        /// </summary>
        /// <param name="id">The user to check against</param>
        /// <returns>Whether the user is authorized or not</returns>
        public static bool IsAuthorized(this UserId id)
        {
            return AuthorizedUsers.Contains(id);
        }

        /// <summary>
        /// Authorizes a user
        /// </summary>
        /// <param name="id">The user to authorize</param>
        public static void Authorize(this UserId id)
        {
            if (!Changeable.Contains(id))
                Changeable.Add(id);
        }

        /// <summary>
        /// Deauthorizes a user
        /// </summary>
        /// <param name="id">The user to deauthorize</param>
        public static void Deauthorize(this UserId id)
        {
            if (Changeable.Contains(id))
                Changeable.Remove(id);
        }

        /// <summary>
        /// Authorizes a list of users
        /// </summary>
        /// <param name="ids">The users to authorize</param>
        public static void Authorize(params UserId[] ids)
        {
            foreach (var id in ids)
                id.Authorize();
        }

        /// <summary>
        /// Deauthorizes a list of users.
        /// </summary>
        /// <param name="ids">The users to deauthorize</param>
        public static void Deauthorize(params UserId[] ids)
        {
            foreach (var id in ids)
                id.Deauthorize();
        }

        /// <summary>
        /// Checks a tagets permissions based on the engine provided
        /// </summary>
        /// <typeparam name="T">The type of permissions engine</typeparam>
        /// <param name="engine">The engine to use to check the permissions</param>
        /// <param name="inQuestion">The required permission to pass the test</param>
        /// <param name="user">The user in question</param>
        /// <param name="group">The group in question (can be null if not a group message)</param>
        /// <returns>Whether or not the target has the required permissions</returns>
        public static bool CheckPermissions<T>(this T engine, Permission inQuestion, User user, Group group) where T : IPermissionEngine
        {
            if (inQuestion == Permission.All)
                return true;

            bool foundPermission = false;
            for (var i = 0; i < engine.PermissionOrder.Length; i++)
            {
                var curPer = engine.PermissionOrder[i];

                if (curPer == Permission.All)
                    continue;
                if (!foundPermission)
                {
                    if (engine.PermissionOrder[i] != inQuestion)
                        continue;
                    foundPermission = true;
                }

                if (!engine.HasPermission.ContainsKey(curPer))
                    continue;

                if (engine.HasPermission[curPer](user, group))
                    return true;
            }

            return false;
        }
    }
}
