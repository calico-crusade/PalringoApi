using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PalringoApi.Networking
{
    /// <summary>
    /// Packet layout for sending messages across the connection
    /// </summary>
    public partial class Packet
    {
        /// <summary>
        /// Holds the message id for the current packet
        /// </summary>
        public long MessageIdHolder = 1; 

        /// <summary>
        /// The command of the packet
        /// </summary>
        public string Command { get; set; } = "";
        /// <summary>
        /// The headers of the packet
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// The payload of the packet
        /// </summary>
        public string Payload { get; set; } = "";

        /// <summary>
        /// Gets or sets the content length of the packet
        /// </summary>
        public int ContentLength
        {
            get
            {
                return Headers.ContainsKey("CONTENT-LENGTH") ?
                    int.Parse(Headers["CONTENT-LENGTH"]) : 0;
            }
            set
            {
                if (Headers.ContainsKey("CONTENT-LENGTH"))
                    Headers["CONTENT-LENGTH"] = value.ToString();
                else
                    Headers.Add("CONTENT-LENGTH", value.ToString());
            }
        }

        /// <summary>
        /// Gets the message id for the packet.
        /// </summary>
        public long MessageId
        {
            get
            {
                if (Headers.ContainsKey("MESG-ID"))
                    return long.Parse(Headers["MESG-ID"]);
                var mid = MessageIdHolder;
                Headers.Add("MESG-ID", mid.ToString());
                MessageIdHolder += 1;
                return mid;
            }
        }

        /// <summary>
        /// Access to the packets headers securly without risk of exception
        /// </summary>
        /// <param name="key">The header key to access</param>
        /// <returns>Null if the header doesn't exist, or the header value if it does exist</returns>
        public string this[string key]
        {
            get
            {
                return Headers.ContainsKey(key) ? Headers[key] : null;
            }
            set
            {
                if (Headers.ContainsKey(key))
                    Headers[key] = value;
                else
                    Headers.Add(key, value);
            }
        }

        /// <summary>
        /// Default constructor for the packet
        /// </summary>
        public Packet() { }

        /// <summary>
        /// Turn the packet into a form palringo can understand
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            //Make sure content-length is present if theres a payload
            if (Payload?.Length > 0 && !Headers.ContainsKey("CONTENT-LENGTH"))
                Headers.Add("CONTENT-LENGTH", Payload.Length.ToString());
            //Make sure message-id is present.
            var mid = MessageId.ToString();
            //Collect the headers
            var headers = string.Join("", Headers.Select(t => $"{t.Key}: {t.Value}\n"));
            //Construct the packet.
            return $"{Command}\n{headers}\n{Payload}";
        }

        /// <summary>
        /// Compress the packet if avialable
        /// </summary>
        /// <returns></returns>
        public string CompressPayload()
        {
            using (var ms = new MemoryStream())
            {
                using (var of = new MemoryStream(Static.PalringoEncoding.GetBytes(Payload)))
                {
                    using (var gs = new ZLibNet.ZLibStream(ms, ZLibNet.CompressionMode.Compress))
                    {
                        of.CopyTo(gs);
                    }
                }
                return Static.PalringoEncoding.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Decompress the packet if avialable
        /// </summary>
        /// <returns></returns>
        public string DecompressPayload()
        {
            using (var msi = new MemoryStream(Static.PalringoEncoding.GetBytes(Payload)))
            using (var mso = new MemoryStream())
            {
                using (var gs = new ZLibNet.ZLibStream(msi, ZLibNet.CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }

                return Static.PalringoEncoding.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// Printable format of the packet
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = string.Format("{0}{1}{2}", Command,
                Headers.Count > 0 ? "\r\n" + string.Join("\r\n", Headers.Select(t => t.Key + ": " + t.Value).ToList()) : "",
                Payload.Length > 0 ? "\r\n" + Payload : "");
            return output;
        }

        /// <summary>
        /// Maps the packet to an object using the packet attributes <see cref="PacketHandling.PacketMap.PacketHeader"/> <see cref="PacketHandling.PacketMap.PacketPayload"/>
        /// </summary>
        /// <typeparam name="T">The type of object to map to</typeparam>
        /// <returns>The mapped object</returns>
        public T Map<T>()
        {
            return PacketHandling.PacketMap.PacketMapper.Map<T>(this);
        }

        /// <summary>
        /// Does a deep clone of the packet
        /// </summary>
        /// <returns>The cloned packet</returns>
        public Packet Clone()
        {
            return new Packet
            {
                Command = Command.ToString(),
                Headers = Headers.ToDictionary(t => t.Key, t => t.Value),
                Payload = Payload.ToString()
            };
        }
    }
}
