using PalringoApi.Subprofile.Types;

namespace PalringoApi.Subprofile
{
    /// <summary>
    /// Inherited version of User that includes their role in group
    /// </summary>
    public class GroupUser : User
    {
        /// <summary>
        /// The users role in group
        /// </summary>
        public Role UserRole { get; set; } = Types.Role.User;

        /// <summary>
        /// The numeric value of the role
        /// </summary>
        public int InterfaceRoleValue
        {
            get
            {
                switch (UserRole)
                {
                    case Role.Owner: return 3;
                    case Role.Admin: return 2;
                    case Role.Mod: return 1;
                    case Role.User: return 0;
                    case Role.Silenced: return -1;
                    case Role.Banned: return -2;
                }
                return 0;
            }
        }

        /// <summary>
        /// The numeric value of the role from a permissions stand point
        /// </summary>
        /// <param name="usrrole"></param>
        /// <returns></returns>
        public static int GetInterfaceRole(Role usrrole)
        {
            switch (usrrole)
            {
                case Role.Owner: return 3;
                case Role.Admin: return 2;
                case Role.Mod: return 1;
                case Role.Banned: return -2;
                case Role.Silenced: return -1;
                case Role.User: return 0;
            }
            return 0;
        }

        /// <summary>
        /// Sets the base user for the class
        /// </summary>
        /// <param name="user"></param>
        public void SetUser(User user)
        {
            Nickname = user.Nickname;
            Status = user.Status;
            Reputation = user.Reputation;
            RepLevel = user.RepLevel;
            Id = user.Id;
            DeviceType = user.DeviceType;
            Privileges = user.Privileges;
        }

        /// <summary>
        /// Wrapper for setting the base user of the class
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static GroupUser FromUser(User user)
        {
            var gm = new GroupUser();
            gm.SetUser(user);
            return gm;
        }
    }
}
