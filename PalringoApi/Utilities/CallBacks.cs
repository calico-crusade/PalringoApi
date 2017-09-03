using PalringoApi.Networking;
using PalringoApi.PacketData;
using System;
using System.Collections.Generic;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// For when something needs <see cref="Packet.Watch(PalBot, Action{Response})"/> 
    /// </summary>
    public class CallBacks
    {
        private List<IReturnCall> ReturnCalls { get; set; } = new List<IReturnCall>();
        
        /// <summary>
        /// Watches a packet, use the wrappers on the <see cref="PalBot"/> class instead of this 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packet"></param>
        /// <param name="of"></param>
        /// <param name="pt"></param>
        /// <param name="m"></param>
        public void Watch<T>(string packet, Action<T> of, Func<Packet, T> pt, Func<T, bool> m)
        {
            var r = new ReturnCall<T>(packet, of, pt, m);
            ReturnCalls.Add(r);
        }

        /// <summary>
        /// Active a callback based on a packet given.
        /// </summary>
        /// <param name="packet"></param>
        public void Activate(Packet packet)
        {
            foreach(var item in ReturnCalls.ToArray())
            {
                if (item.Process(packet))
                    ReturnCalls.Remove(item);
            }
        }
    }
}
