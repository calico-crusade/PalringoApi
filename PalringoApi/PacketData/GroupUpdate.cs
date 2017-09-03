using PalringoApi.Subprofile;

namespace PalringoApi.PacketData
{
    /// <summary>
    /// Holder for a group update
    /// </summary>
    public class GroupUpdate
    {
        /// <summary>
        /// The group that was updated.
        /// </summary>
        public GroupId Group { get; set; }

        /// <summary>
        /// The user that was updated.
        /// </summary>
        public UserId User { get; set; }
        
        /// <summary>
        /// Whether the update was a join or a leave.
        /// </summary>
        public bool IsJoin { get; set; }
    }
}
