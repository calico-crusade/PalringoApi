using PalringoApi.Networking;
using PalringoApi.Networking.PacketHandling.PacketMap;
using PalringoApi.Subprofile.Types;
using PalringoApi.Utilities;
using System;
using System.Collections.Generic;

namespace PalringoApi.PacketData
{
    /// <summary>
    /// A holder for a message packet.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// When a response is obtained
        /// </summary>
        public Action<Response> OnResponse { set; get; }

        #region Properties
        /// <summary>
        /// The user who sent the message
        /// </summary>
        [PacketHeader("SOURCE-ID")]
        public int SourceId { get; set; }

        /// <summary>
        /// The target group of the message
        /// </summary>
        [PacketHeader("TARGET-ID")]
        public int? TargetId { get; set; }

        /// <summary>
        /// The message contents (payload)
        /// </summary>
        [PacketPayload]
        public string Content { get; set; }

        /// <summary>
        /// The string representation of the type of message
        /// </summary>
        [PacketHeader("CONTENT-TYPE")]
        public string MimeType { get; set; }

        /// <summary>
        /// The string representation of the time stamp.
        /// </summary>
        [PacketHeader("TIMESTAMP")]
        public string UnsortedTimestamp { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Message() { }

        /// <summary>
        /// Constrcutor for string data
        /// </summary>
        /// <param name="target">Target ID</param>
        /// <param name="mt">The message type</param>
        /// <param name="dt">The data type</param>
        /// <param name="data">The data</param>
        public Message(int target, MessageType mt, DataType dt, string data)
        {
            SourceId = 0;
            TargetId = null;
            UnsortedTimestamp = DateTime.Now.Ticks.ToString();
            if (mt == MessageType.Group)
                TargetId = target;
            else
                SourceId = target;
            MimeType = dt.FromDataType();
            Content = data;
        }

        /// <summary>
        /// Constructor for byte[] data
        /// </summary>
        /// <param name="target">The target id</param>
        /// <param name="mt">The message type</param>
        /// <param name="dt">The data type</param>
        /// <param name="data">the data</param>
        public Message(int target, MessageType mt, DataType dt, byte[] data)
        {
            SourceId = 0;
            TargetId = null;
            UnsortedTimestamp = DateTime.Now.Ticks.ToString();
            if (mt == MessageType.Group)
                TargetId = target;
            else
                SourceId = target;
            MimeType = dt.FromDataType();
            Content = Static.PalringoEncoding.GetString(data);
        }
        #endregion

        #region Auto Properties
        /// <summary>
        /// The type of message (Group or otherwise)
        /// </summary>
        public MessageType MesgType => TargetId == null ? MessageType.Private : MessageType.Group;

        /// <summary>
        /// The DateTime representation of the time stamp.
        /// </summary>
        public DateTime Timestamp => UnsortedTimestamp.FromPalUnix();

        /// <summary>
        /// The Content Type of the message.
        /// </summary>
        public DataType ContentType => MimeType.FromMimeType();

        /// <summary>
        /// The targeted group's ID (only set if the message is a group message)
        /// </summary>
        public GroupId GroupId => TargetId.HasValue ? TargetId.Value : -1;

        /// <summary>
        /// The user who sent the message.
        /// </summary>
        public UserId SourceUser => SourceId;

        /// <summary>
        /// The group ID or user ID depending on the message type.
        /// </summary>
        public int ReturnAddress => MesgType == MessageType.Private ? SourceId : TargetId.Value;
        #endregion

        /// <summary>
        /// Obtains the serialized packet form of the messages.
        /// </summary>
        /// <returns></returns>
        public Packet[] Package()
        {
            return Package(this);
        }

        /// <summary>
        /// Does a deep clone of the object
        /// </summary>
        /// <returns></returns>
        public Message Clone()
        {
            return new Message
            {
                SourceId = SourceId,
                Content = Content,
                MimeType = MimeType,
                TargetId = TargetId,
                UnsortedTimestamp = UnsortedTimestamp
            };
        }

        /// <summary>
        /// Packages up the data
        /// </summary>
        /// <param name="mesg"></param>
        /// <returns></returns>
        public static Packet[] Package(Message mesg)
        {
            return Package(mesg.MesgType, mesg.ContentType, mesg.ReturnAddress, mesg.Content);
        }

        /// <summary>
        /// Packages up the data
        /// </summary>
        /// <param name="mesgtype"></param>
        /// <param name="dataType"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Packet[] Package(MessageType mesgtype, DataType dataType, int id, string data)
        {
            return PacketChunkanizer.Chunk(new Packet
            {
                Command = "MESG",
                Headers = new Dictionary<string, string>
                {
                    ["Target-Id"] = id.ToString(),
                    ["Mesg-Target"] = ((int)mesgtype).ToString(),
                    ["Content-Type"] = dataType.FromDataType()
                }
            }, data);
        }

        /// <summary>
        /// Packes up the data
        /// </summary>
        /// <param name="mesgtype"></param>
        /// <param name="dataType"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Packet[] Package(MessageType mesgtype, DataType dataType, int id, byte[] data)
        {
            return Package(mesgtype, dataType, id, Static.PalringoEncoding.GetString(data));
        }
    }
}
