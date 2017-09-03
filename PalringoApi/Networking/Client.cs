using System;
using System.Net.Sockets;
using System.Text;

namespace PalringoApi.Networking
{
    /// <summary>
    /// The network client for the TCP back end
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The default IP address for palringo (can be load balancer)
        /// </summary>
        public static string DefaultHost = "im.palringo.com";
        /// <summary>
        /// The default port for palringo (just leave alone, tbh)
        /// </summary>
        public static int DefaultPort = 12345;

        /// <summary>
        /// When the TCP client makes a connection
        /// </summary>
        public DataCarrier OnConnected = delegate { };
        /// <summary>
        /// When the TCP client disconnects
        /// </summary>
        public DataCarrier OnDisconnected = delegate { };
        /// <summary>
        /// When the TCP client fails to connect
        /// </summary>
        public DataCarrier OnConnectFailed = delegate { };
        /// <summary>
        /// When a Packet has been parsed and recieved.
        /// </summary>
        public DataCarrier<Packet> OnPacketReceived = delegate { };
        /// <summary>
        /// When a packet has been sent
        /// </summary>
        public DataCarrier<Packet> OnPacketSent = delegate { };

        /// <summary>
        /// The host for the current connection instance
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// The port for the current connection instance
        /// </summary>
        public int Port { get; private set; }

        private TcpClient _client;
        private byte[] _buffer;
        private PacketParser _parser;

        private NetworkStream _stream => _client.GetStream();
        /// <summary>
        /// Tells whether the client is connection or not
        /// </summary>
        public bool isConnected => _client != null && _client.Connected;

        /// <summary>
        /// Default Constructor (incase any are added later)
        /// </summary>
        public Client() { }

        /// <summary>
        /// Starts the connection to palringo
        /// </summary>
        /// <param name="host">The optional host for the connection</param>
        /// <param name="port">The optional port for the connection</param>
        public void Start(string host = null, int? port = null)
        {
            try
            {
                Host = string.IsNullOrEmpty(host) ? DefaultHost : host;
                Port = port == null ? DefaultPort : port.Value;
                _client = new TcpClient();
                _parser = new PacketParser();
                _parser.PacketParsed += (a) =>
                {
                    OnPacketReceived(a);
                };
                _client.BeginConnect(Host, Port, ConnectionCallBack, null);
            }
            catch
            {
                OnConnectFailed();
            }
        }
        /// <summary>
        /// Disconnect the TCP client
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _client.Close();
            }
            catch { }
        }
        /// <summary>
        /// Write a packet to the TCP Connection
        /// </summary>
        /// <param name="packet">The packet to write</param>
        public void WritePacket(Packet packet)
        {
            try
            {
                var output = Static.PalringoEncoding.GetBytes(packet.Serialize());
                _stream.BeginWrite(output, 0, output.Length, WriteCallBack, packet);
            }
            catch { }
        }
        /// <summary>
        /// Writes multiple packets to the TCP Connection
        /// </summary>
        /// <param name="packets"></param>
        public void WritePackets(params Packet[] packets)
        {
            foreach (var pack in packets)
                WritePacket(pack);
        }

        private void ConnectionCallBack(IAsyncResult res)
        {
            try
            {
                if (!isConnected)
                {
                    OnConnectFailed();
                    return;
                }
                _buffer = new byte[_client.ReceiveBufferSize];
                _stream.BeginRead(_buffer, 0, _buffer.Length, ReadCallBack, null);
                OnConnected();
            }
            catch
            {
                OnConnectFailed();
            }
        }
        private void ReadCallBack(IAsyncResult res)
        {
            if (!_client.Connected)
            {
                OnDisconnected();
                return;
            }

            int length;
            try
            {
                if ((length = _stream.EndRead(res)) == 0)
                {
                    OnDisconnected();
                    return;
                }
            }
            catch
            {
                OnDisconnected();
                return;
            }

            string data = Static.PalringoEncoding.GetString(_buffer, 0, length);
            
            try
            {
                _parser.Process(data);
            }
            catch (Exception ex) {  }

            if (_client.Connected)
            {
                try
                {
                    _stream.BeginRead(_buffer, 0, _buffer.Length, ReadCallBack, null);
                }
                catch  
                {
                    OnDisconnected();
                }
            }
        }
        private void WriteCallBack(IAsyncResult res)
        {
            try
            {
                _stream.EndWrite(res);
                var packet = (Packet)res.AsyncState;
                OnPacketSent(packet);
            }
            catch { }
        }
    }
}
