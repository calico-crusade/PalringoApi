using PalringoApi.Networking;
using PalringoApi.PacketData;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// Creates a queue to store messages and processes them in order to stop throttling
    /// </summary>
    public class MessageQueue
    {
        private bool _watch { get; set; } = true;

        private List<Message> _queue { get; set; } = new List<Message>();

        private PalBot _bot { get; set; }

        private Client _client { get; set; }

        private int _wait { get; set; } = 0;

        private bool _working { get; set; } = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="watchThrottle"></param>
        /// <param name="cli"></param>
        public MessageQueue(PalBot bot, bool watchThrottle, Client cli)
        {
            _bot = bot;
            _watch = watchThrottle;
            _client = cli;
            bot.On.Throttle += (r) => _wait = r;
        }

        /// <summary>
        /// Puts a message on the Queueu
        /// </summary>
        /// <param name="msg"></param>
        public void Drop(Message msg)
        {
            if (!_watch)
            {
                _client.WritePackets(msg.Package().Watch(_bot, msg.OnResponse));
                return;
            }

            _queue.Add(msg);
            CheckWorker();
        }

        private void CheckWorker()
        {
            if (_working)
                return;

            _working = true;
            OnCallBack(null, null);
        }
        
        private void OnCallBack(Action<Response> onFinish, Response resp)
        {
            if (_queue.Count <= 0)
            {
                _working = false;
                _wait = 0;
                onFinish?.Invoke(resp);
                return;
            }
            if (_wait > 0)
            {
                new Thread(() =>
                {
                    Thread.Sleep(_wait * 1000);
                    _wait = 0;
                    OnCallBack(onFinish, resp);
                }).Start();
                return;
            }
            onFinish?.Invoke(resp);
            var next = _queue[0];
            var packets = next.Package().Watch(_bot, (r) => OnCallBack(next.OnResponse, r));
            _client.WritePackets(packets);
            _queue.Remove(next);
        }
    }
} 