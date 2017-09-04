using PalringoApi.PacketData;

namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles when an admin action packet is obtained
    /// </summary>
    public class AdminActionHandler : PacketHandler
    {
        /// <summary>
        /// The command "GROUP ADMIN"
        /// </summary>
        public override string Command => "GROUP ADMIN";

        /// <summary>
        /// What happens when the packet is processed
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            var action = Map<AdminAction>(packet);
            UpdateGroup(action);

            Bot.On.Trigger("aa", action);
        }

        private void UpdateGroup(AdminAction action)
        {
            var group = action.GroupId.GetGroup(Bot);
            if (!group.Exists)
                return;

            if (!group.Members.ContainsKey(action.SourceId) || !group.Members.ContainsKey(action.TargetId))
                return;

            var userRole = action.Action.ToRole();
            group.Members[action.TargetId].UserRole = userRole;
        }
    }
}
