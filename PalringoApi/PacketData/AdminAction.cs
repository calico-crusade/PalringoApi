using PalringoApi.Networking.PacketHandling.PacketMap;
using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Types;

namespace PalringoApi.PacketData
{
    /// <summary>
    /// Holder for an admin action.
    /// </summary>
    public class AdminAction
    {
        /// <summary>
        /// The person who preformed the admin action.
        /// </summary>
        [PacketHeader("SOURCE-ID")]
        public UserId SourceId { get; set; }
        /// <summary>
        /// The person who the action was preformed upon.
        /// </summary>
        [PacketHeader("TARGET-ID")]
        public UserId TargetId { get; set; }
        /// <summary>
        /// The Group the action was preformed in.
        /// </summary>
        [PacketHeader("GROUP-ID")]
        public GroupId GroupId { get; set; }
        /// <summary>
        /// The action that was preformed.
        /// </summary>
        [PacketHeader("ACTION")]
        public AdminActions Action { get; set; }
    }
}
