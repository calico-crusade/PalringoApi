using System.Collections.Generic;
using System.Linq;

namespace PalringoApi.Networking
{
    /// <summary>
    /// Chunks packets to allow for massive data sending
    /// </summary>
    public static class PacketChunkanizer
    { 
        /// <summary>
        /// The packet to split up and chunk
        /// </summary>
        /// <param name="packet">The packet to chunk</param>
        /// <param name="data">The data to put into chunks</param>
        /// <param name="chunkSize">How big the chunks should be</param>
        /// <returns>The array of packets that have been chunked</returns>
        public static Packet[] Chunk(Packet packet, string data, int chunkSize = 512)
        {
            var split = data.SplitChunks(chunkSize).ToArray();
            if (split.Length <= 0)
                return new Packet[] { packet };

            if (split.Length == 1)
                return new[] { StandAlone(packet, data) };

            var p = new List<Packet>();
            long corid = 0;
            for (var i = 0; i < split.Length; i++)
            {
                if (i == 0)
                {
                    var head = Head(packet, split[i], data.Length);
                    corid = head.MessageId;
                    p.Add(head);
                    continue;
                }

                if (i + 1 >= split.Length)
                {
                    p.Add(Last(packet, split[i], corid));
                    continue;
                }

                p.Add(Mid(packet, split[i], corid));
            }

            return p.ToArray();
        }

        /// <summary>
        /// Chunks a packet using the data in the payload
        /// </summary>
        /// <param name="packet">The packet to chunk</param>
        /// <param name="chunkSize">The size of the chunks to make</param>
        /// <returns>The array of chunked packets.</returns>
        public static Packet[] Chunk(Packet packet, int chunkSize = 512)
        {
            return Chunk(packet, packet.Payload, chunkSize);
        }

        private static Packet StandAlone(Packet packet, string data)
        {
            var p = packet.Clone();
            p["total-length"] = data.Length.ToString();
            p["last"] = "T";
            p.Payload = data;
            return p;
        }

        private static Packet Head(Packet packet, string chunk, int total)
        {
            var p = packet.Clone();
            p["total-length"] = total.ToString();
            p.Payload = chunk;
            return p;
        }

        private static Packet Mid(Packet packet, string chunk, long corid)
        {
            var p = packet.Clone();
            p["correlation-id"] = corid.ToString();
            p.Payload = chunk;
            return p;
        }

        private static Packet Last(Packet packet, string chunk, long corid)
        {
            var m = Mid(packet, chunk, corid);
            m["last"] = "1";
            return m;
        }
    }
}
