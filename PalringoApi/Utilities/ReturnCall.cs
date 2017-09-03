using PalringoApi.Networking;
using System;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// Stores a call back from a Return call object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReturnCall<T> : IReturnCall
    {
        /// <summary>
        /// The packet command to watch for
        /// </summary>
        public string PacketToWatch { get; set; }
        /// <summary>
        /// What to do when its finished
        /// </summary>
        public Action<T> OnFinished { get; set; }
        /// <summary>
        /// The translation from the packet to the object
        /// </summary>
        public Func<Packet, T> PackTrans { get; set; }
        /// <summary>
        /// If the object that was translated matches the call creteria
        /// </summary>
        public Func<T, bool> Matches { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p"></param>
        /// <param name="of"></param>
        /// <param name="pt"></param>
        /// <param name="m"></param>
        public ReturnCall(string p, Action<T> of, Func<Packet, T> pt, Func<T, bool> m)
        {
            PacketToWatch = p;
            OnFinished = of;
            PackTrans = pt;
            Matches = m;
        }

        /// <summary>
        /// Process a packet
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public bool Process(Packet packet)
        {
            if (packet.Command.ToUpper() != PacketToWatch.ToUpper())
                return false;
            var t = PackTrans(packet.Clone());
            if (!Matches(t))
                return false;
            OnFinished(t);
            return true;
        }
    }

    /// <summary>
    /// Interface for an ReturnCall thing
    /// </summary>
    public interface IReturnCall
    {
        /// <summary>
        /// Processing method
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        bool Process(Packet packet);
    }
}
