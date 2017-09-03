using PalringoApi.Networking.PacketHandling.PacketMap;
using PalringoApi.Subprofile.Types;
using PalringoApi.Utilities;

namespace PalringoApi.PacketData
{
    /// <summary>
    /// A holder for the response packet
    /// </summary>
    public class Response
    {
        /// <summary>
        /// What type of response is it.
        /// </summary>
        [PacketHeader("WHAT")]
        public What What { get; set; }
        /// <summary>
        /// Whether its a code or a message
        /// </summary>
        [PacketHeader("TYPE")]
        public Type Type { get; set; }
        /// <summary>
        /// The message id the response is for.
        /// </summary>
        [PacketHeader("MESG-ID")]
        public long MessageId { get; set; }
        /// <summary>
        /// The message (only set if type is of message)
        /// </summary>
        [PacketPayload]
        public string Message { get; set; }
        /// <summary>
        /// The code (only set if type is of code)
        /// </summary>
        public Code Code => Type == Type.Code ? (Code)DataInputStream.GetLong(Message) : Code.INTERNAL_CODE;

        /// <summary>
        /// Printable version of the Reponse data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{What.ToString()}[{MessageId}]: {(Type == Type.Code ? Code.ToString() : Message)}"; 
        }
    }
}
