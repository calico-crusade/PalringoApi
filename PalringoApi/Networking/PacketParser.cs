using System.Linq;

namespace PalringoApi.Networking
{
    /// <summary>
    /// Packet parser turns the byte stream into packets
    /// </summary>
    public class PacketParser
    {
        /// <summary>
        /// The event that is triggered on a packet being parsed
        /// </summary>
        public event DataCarrier<Packet> PacketParsed = delegate { };

        /// <summary>
        /// Start processing a packet
        /// </summary>
        /// <param name="data">The packet data</param>
        public void Process(string data)
        {
            if (string.IsNullOrEmpty(_overflow))
                ProcessPacket(data);
            else
                ProcessPacket(_overflow + data);
        }

        private string _overflow;

        private void ProcessPacket(string data)
        {
            Packet packet = new Packet();
            if (string.IsNullOrEmpty(data))
                return;
            string cph = data.Substring(0, data.IndexOf("\r\n\r\n") == -1 ? data.Length : data.IndexOf("\r\n\r\n"));
            if (!cph.Contains("\r\n"))
            {
                packet.Command = cph;
                PacketParsed(packet);
                _overflow = null;
                return;
            }
            packet.Command = cph.Substring(0, cph.IndexOf("\r\n"));
            string headers = cph.Substring(cph.IndexOf("\r\n") + 2, cph.Length - cph.IndexOf("\r\n") - 2);
            packet = DeserializeHeaders(headers, packet);
            if (packet.ContentLength == 0 && data.Length == cph.Length + 2)
            {
                PacketParsed(packet);
                _overflow = null;
                return;
            }
            else if (packet.ContentLength == 0)
            {
                PacketParsed(packet);
                try
                {
                    _overflow = data.Remove(0, cph.Length + 4);
                }
                catch
                {
                    _overflow = null;
                }
                return;
            }
            string payload = data.Substring(data.IndexOf("\r\n\r\n") + 4, data.Length - data.IndexOf("\r\n\r\n") - 4);
            packet.Payload = payload;
            if (packet.Payload.Length < packet.ContentLength)
            {
                _overflow = data;
                return;
            }
            if (packet.Payload.Length > packet.ContentLength)
            {
                _overflow = packet.Payload.Remove(0, packet.ContentLength);
                packet.Payload = packet.Payload.Substring(0, packet.ContentLength);
                PacketParsed(packet);
                ProcessPacket(_overflow);
                return;
            }
            PacketParsed(packet);
            _overflow = null;
        }

        private Packet DeserializeHeaders(string headers, Packet packet)
        {
            var pack = new Packet();
            pack.Command = packet.Command;
            var lines = headers.Split('\n').ToList();
            foreach (var line in lines)
            {
                if (!line.Contains(':'))
                    continue;
                string key = line.Substring(0, line.IndexOf(':')).ToUpper();
                string value = line.Remove(0, key.Length + 1);
                pack.Headers.Add(key.Trim(), value.Trim());
            }
            return pack;
        }
    }
}
