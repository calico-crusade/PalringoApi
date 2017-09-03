using Newtonsoft.Json;
using PalringoApi.Networking;
using PalringoApi.PacketData;
using PalringoApi.PluginManagement.Permissions;
using PalringoApi.Subprofile;
using System;
using System.Collections.Generic;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// These are subscribable events that active when things happen
    /// </summary>
    public class Events
    {
        /// <summary>
        /// When the bot connects
        /// </summary>
        public event DataCarrier Connected = delegate { };
        /// <summary>
        /// When the bot disconnects
        /// </summary>
        public event DataCarrier Disconnected = delegate { };
        /// <summary>
        /// When the bot fails to connect
        /// </summary>
        public event DataCarrier ConnectFailed = delegate { };
        /// <summary>
        /// When the bot gets Ghosted (someone else logs in)
        /// </summary>
        public event DataCarrier Ghosted = delegate { };
        /// <summary>
        /// When the bot logs in successfully
        /// </summary>
        public event DataCarrier LoginSuccess = delegate { };
        /// <summary>
        /// When the bot Throttles (delay)
        /// </summary>
        public event DataCarrier<int> Throttle = delegate { };
        /// <summary>
        /// When the bot fails to login (reason)
        /// </summary>
        public event DataCarrier<string> LoginFailed = delegate { };
        /// <summary>
        /// When the bot throws an exception (error)
        /// </summary>
        public event DataCarrier<Exception> Error = delegate { };
        /// <summary>
        /// When the bot recieves a group message (message)
        /// </summary>
        public event DataCarrier<Message> GroupMessage = delegate { };
        /// <summary>
        /// When the bot recieves a private message (message)
        /// </summary>
        public event DataCarrier<Message> PrivateMessage = delegate { };
        /// <summary>
        /// When the bot sees an admin action (action)
        /// </summary>
        public event DataCarrier<AdminAction> GroupAdmin = delegate { };
        /// <summary>
        /// When the bot joins a group (Group profile)
        /// </summary>
        public event DataCarrier<Group> GroupJoined = delegate { };
        /// <summary>
        /// When the bot leaves a group (Group profile) (forced or otherwise)
        /// </summary>
        public event DataCarrier<Group> GroupLeft = delegate { };
        /// <summary>
        /// When the bot sees someone else joining or leaving (updateinfo)
        /// </summary>
        public event DataCarrier<GroupUpdate> GroupUpdate = delegate { };
        /// <summary>
        /// When a debug statement is logged in the bot (mostly for my use)
        /// </summary>
        public event DataCarrier<string> Log = delegate { };
        /// <summary>
        /// When a packet is not processed by the packet parser.
        /// </summary>
        public event DataCarrier<Packet> UnprocessedPacket = delegate { };
        /// <summary>
        /// When a packet is received by the system.
        /// </summary>
        public event DataCarrier<Packet> PacketReceieved = delegate { };
        /// <summary>
        /// When a packet has been sent by the system.
        /// </summary>
        public event DataCarrier<Packet> PacketSent = delegate { };
        /// <summary>
        /// When a user fails a permissions test (Requested by Dave)
        /// </summary>
        public event DataCarrier<PalBot, FailedPermissionsReport> PermissionsFailed = delegate { };

        private Dictionary<string, Action<object, object>> _events;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Events()
        {
            _events = new Dictionary<string, Action<object, object>>
            {
                ["c"] = (a, b) => Connected(),
                ["d"] = (a, b) => Disconnected(),
                ["e"] = (a, b) => Error((Exception)a),
                ["cf"] = (a, b) => ConnectFailed(),
                ["gh"] = (a, b) => Ghosted(),
                ["ls"] = (a, b) => LoginSuccess(),
                ["lf"] = (a, b) => LoginFailed((string)a),
                ["gm"] = (a, b) => GroupMessage((Message)a),
                ["pm"] = (a, b) => PrivateMessage((Message)a),
                ["aa"] = (a, b) => GroupAdmin((AdminAction)a),
                ["gj"] = (a, b) => GroupJoined((Group)a),
                ["gl"] = (a, b) => GroupLeft((Group)a),
                ["gu"] = (a, b) => GroupUpdate((GroupUpdate)a),
                ["ll"] = (a, b) => Log((string)a),
                ["th"] = (a, b) => Throttle((int)a),
                ["pk"] = (a, b) => UnprocessedPacket((Packet)a),
                ["pf"] = (a, b) => PermissionsFailed((PalBot)a, (FailedPermissionsReport)b),
                ["pr"] = (a, b) => PacketReceieved((Packet)a),
                ["ps"] = (a, b) => PacketSent((Packet)a)
            };
        }

        private void _trigger(string name, object o1, object o2)
        {
            try
            {
                if (_events.ContainsKey(name))
                    _events[name](o1, o2);
            }
            catch (Exception ex)
            {
                Error(new Exception(ErrorLog("Event Error-" + name + "-59", ex, o1, o2)));
            }
        }
        
        /// <summary>
        /// Triggers an event with no parameters
        /// </summary>
        /// <param name="name"></param>
        public void Trigger(string name)
        {
            try
            { 
                _trigger(name, null, null);
            }
            catch (Exception ex)
            {
                Error(new Exception(ErrorLog("Event Error-" + name + "-59", ex)));
            }
        }

        /// <summary>
        /// Triggers an event with 1 parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="arg1"></param>
        public void Trigger<T>(string name, T arg1)
        {
            try
            { 
                _trigger(name, arg1, null);
            }
            catch (Exception ex)
            {
                Error(new Exception(ErrorLog("Event Error-" + name + "-59", ex, arg1)));
            }
        }

        /// <summary>
        /// Triggers an event with 2 parameters
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="name"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void Trigger<T1, T2>(string name, T1 arg1, T2 arg2)
        {
            try
            {
                _trigger(name, arg1, arg2);
            }
            catch (Exception ex)
            {
                Error(new Exception(ErrorLog("Event Error-" + name + "-59", ex, arg1, arg2)));
            }
        }

        private string ErrorLog(string title, Exception ex, params object[] references)
        {
            var obj = "";
            for(var i = 0; i < references.Length; i++)
            {
                var o = references[i];
                obj += "\r\n" + (o == null ?
                    $"Object {i + 1} is null." :
                    $"Object {i + 2}: {JsonConvert.SerializeObject(o, Formatting.Indented)}");
            }

            var dt = DateTime.Now;
            return $@"Error [{title}]{obj}
Message: {ex.Message}
Help Link: {ex.HelpLink}
Stack Trace:
{ex.ToString()}";
        }
    }
}
