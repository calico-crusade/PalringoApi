using Newtonsoft.Json;
using PalringoApi.PacketData;
using PalringoApi.Subprofile;
using PalringoApi.Utilities;
using System.Drawing;
using System.Linq;
using rs = System.Action<PalringoApi.PacketData.Response>;

namespace PalringoApi.PluginManagement
{
    /// <summary>
    /// Includes packet senders from an instanced Message
    /// </summary>
    public abstract class SendersInstance
    {
        #region Properties
        /// <summary>
        /// The bot instance
        /// </summary>
        [JsonIgnore]
        public PalBot Bot { get; set; }
        
        /// <summary>
        /// The most recent message sent
        /// </summary>
        [JsonIgnore]
        public Message Message { get; set; }

        /// <summary>
        /// The group the message was sent in
        /// </summary>
        [JsonIgnore]
        public Group SourceGroup => Message.GroupId.GetGroup(Bot);

        /// <summary>
        /// The user who sent the message
        /// </summary>
        [JsonIgnore]
        public User SourceUser => Message.SourceUser.GetUser(Bot);

        /// <summary>
        /// Provides the language the current plugin is using.
        /// </summary>
        [JsonIgnore]
        public abstract string Language { get; set; }
        #endregion

        #region packet senders
        /// <summary>
        /// Replies to the message at hand with a string.
        /// </summary>
        /// <param name="data">The message to be sent</param>
        /// <param name="onResponse">The optional response handler</param>
        public void Reply(string data, rs onResponse = null)
        {
            Bot.Reply(Message, data, onResponse);
        }

        /// <summary>
        /// Replies to the message at hand with a string
        /// </summary>
        /// <param name="data">The message to be sent</param>
        /// <returns>The message response</returns>
        public Response ReplyAsync(string data)
        {
            return Bot.Reply(Message, data).Result;
        }

        /// <summary>
        /// Replies to the message at hand with an image.
        /// </summary>
        /// <param name="data">The image to be sent</param>
        /// <param name="onResponse">The optional response handler</param>
        public void Reply(byte[] data, rs onResponse = null)
        {
            Bot.Reply(Message, data, onResponse);
        }

        /// <summary>
        /// Replies to the message at hand with an image.
        /// </summary>
        /// <param name="data">The image to be sent</param>
        /// <param name="onResponse">The optional response handler</param>
        public void Reply(Bitmap data, rs onResponse = null)
        {
            Bot.Reply(Message, data, onResponse);
        }

        /// <summary>
        /// Replies to the message at hand with an image from a local or web resource.
        /// </summary>
        /// <param name="uri">The path to the image</param>
        /// <param name="isWeb">Whether it is a web image or a local one</param>
        /// <param name="onResponse">The optional response handler</param>
        public void Reply(string uri, bool isWeb, rs onResponse = null)
        {
            Bot.Reply(Message, uri, isWeb, onResponse);
        }

        /// <summary>
        /// Replies to the message at hand with a voice message.
        /// </summary>
        /// <param name="data">The image to be sent</param>
        /// <param name="onResponse">The optional response handler</param>
        public void Reply(VoiceMessage data, rs onResponse = null)
        {
            Bot.Reply(Message, data, onResponse);
        }

        /// <summary>
        /// Send a private message to a user
        /// </summary>
        /// <param name="id">The user to send it to</param>
        /// <param name="data">The message to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void PrivateMessage(int id, string data, rs OnResponse = null)
        {
            Bot.PrivateMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Sends a private message to a user
        /// </summary>
        /// <param name="id">The user to send it to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The message response</returns>
        public Response PrivateMessageAsync(int id, string data)
        {
            return Bot.PrivateMessage(id, data).Result;
        }

        /// <summary>
        /// Send a private message to a user
        /// </summary>
        /// <param name="id">The user to send it to</param>
        /// <param name="data">The image to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void PrivateMessage(int id, byte[] data, rs OnResponse = null)
        {
            Bot.PrivateMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Send a private message to a user
        /// </summary>
        /// <param name="id">The user to send it to</param>
        /// <param name="data">The image to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void PrivateMessage(int id, Bitmap data, rs OnResponse = null)
        {
            Bot.PrivateMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Send a private message to a user
        /// </summary>
        /// <param name="id">The user to send it to</param>
        /// <param name="data">The image to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void PrivateMessage(int id, VoiceMessage data, rs OnResponse = null)
        {
            Bot.PrivateMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Sends a private message to a user
        /// </summary>
        /// <param name="id">The user to send it to</param>
        /// <param name="uri">The path to the image</param>
        /// <param name="isWeb">Whether its a web image or local one</param>
        /// <param name="onResponse">The optional response handler</param>
        public void PrivateMessage(int id, string uri, bool isWeb, rs onResponse = null)
        {
            Bot.PrivateMessage(id, uri, isWeb, onResponse);
        }

        /// <summary>
        /// Sends a group message
        /// </summary>
        /// <param name="id">The group to send it to</param>
        /// <param name="data">The message to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void GroupMessage(int id, string data, rs OnResponse = null)
        {
            Bot.GroupMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Sends a group message
        /// </summary>
        /// <param name="id">The group to send it to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The message response</returns>
        public Response GroupMessageAsync(int id, string data)
        {
            return Bot.GroupMessage(id, data).Result;
        }

        /// <summary>
        /// Sends a group message
        /// </summary>
        /// <param name="id">The group to send it to</param>
        /// <param name="data">The image to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void GroupMessage(int id, byte[] data, rs OnResponse = null)
        {
            Bot.GroupMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Sends a group message
        /// </summary>
        /// <param name="id">The group to send it to</param>
        /// <param name="data">The image to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void GroupMessage(int id, Bitmap data, rs OnResponse = null)
        {
            Bot.GroupMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Sends a group message
        /// </summary>
        /// <param name="id">The group to send it to</param>
        /// <param name="data">The image to send</param>
        /// <param name="OnResponse">The optional response handler</param>
        public void GroupMessage(int id, VoiceMessage data, rs OnResponse = null)
        {
            Bot.GroupMessage(id, data, OnResponse);
        }

        /// <summary>
        /// Sends a group message
        /// </summary>
        /// <param name="id">The group to send it to</param>
        /// <param name="uri">The path to the image</param>
        /// <param name="isWeb">Whether it is a web image or local one</param>
        /// <param name="onResponse">The optional response handler</param>
        public void GroupMessage(int id, string uri, bool isWeb, rs onResponse = null)
        {
            Bot.GroupMessage(id, uri, isWeb, onResponse);
        }

        /// <summary>
        /// Leaves the group via the specified ID
        /// </summary>
        /// <param name="id">The group to leave.</param>
        public async void LeaveGroup(GroupId id)
        {
            await Bot.LeaveGroup(id);
        }

        /// <summary>
        /// Joins the sepcified group.
        /// </summary>
        /// <param name="name"></param>
        public async void JoinGroup(string name)
        {
            await Bot.JoinGroup(name);
        }

        /// <summary>
        /// Joins the sepcified group.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        public async void JoinGroup(string name, string pwd)
        {
            await Bot.JoinGroup(name, pwd);
        }

        /// <summary>
        /// Gets a user by id
        /// </summary>
        /// <param name="id">The Id of the user to get</param>
        /// <returns>The user of the id</returns>
        public User GetUser(UserId id)
        {
            return id.GetUser(Bot);
        }

        /// <summary>
        /// Gets a group by id
        /// </summary>
        /// <param name="id">The Id of the group to get</param>
        /// <returns>The group of the id</returns>
        public Group GetGroup(GroupId id)
        {
            return id.GetGroup(Bot);
        }

        #endregion

        /// <summary>
        /// Translates the given key with the language currently on file with the default as a fall back
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public string Translate(string key, string def = null)
        {
            if (Language == null || key == null)
                return def;

            var trans = Bot.Translations[key]
                .Where(t => t.Language == Language)
                .ToArray();

            if (trans.Length <= 0)
                return def;
            return trans[0].Value;
        }
    }
}
