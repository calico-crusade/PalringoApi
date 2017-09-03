using PalringoApi.PacketData;
using PalringoApi.Plugins.Forums;
using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Types;
using PalringoApi.Utilities;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PalringoApi.Plugins
{
    /// <summary>
    /// Instance of IIForum to provider helper methods
    /// </summary>
    public abstract class IForum : IIForum
    {
        /// <summary>
        /// Instance of the forum attribute
        /// </summary>
        public Forum AttributeInstance { get; set; }

        /// <summary>
        /// Instance of the bot
        /// </summary>
        public PalBot Bot { get; set; }

        /// <summary>
        /// language the current forum is in
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The message that was recieved.
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// whether or not the Forum has been canceled
        /// </summary>
        public bool Canceled { get; private set; } = false;

        private TaskCompletionSource<Message> _tcs;

        #region Methods

        #region Reply
        /// <summary>
        /// Replies to the message at hand with a string
        /// </summary>
        /// <param name="data">The Message to send</param>
        /// <returns>The response of the message</returns>
        public Response Reply(string data)
        {
            return Bot.Reply(Message, data).Result;
        }

        /// <summary>
        /// Replies to the message at hand with a string
        /// </summary>
        /// <param name="data">The Message to send</param>
        /// <returns>The response of the message</returns>
        public Response Reply(Bitmap data)
        {
            return Bot.Reply(Message, data).Result;
        }

        /// <summary>
        /// Replies to the message at hand with a string
        /// </summary>
        /// <param name="data">The Message to send</param>
        /// <returns>The response of the message</returns>
        public Response Reply(byte[] data)
        {
            return Bot.Reply(Message, data).Result;
        }

        /// <summary>
        /// Replies to the message at hand with a string
        /// </summary>
        /// <param name="data">The Message to send</param>
        /// <param name="isWeb">Whether or not its a web request</param>
        /// <returns>The response of the message</returns>
        public Response Reply(string data, bool isWeb)
        {
            return Bot.Reply(Message, data, isWeb).Result;
        }

        /// <summary>
        /// Short cut to Reply with Translation
        /// </summary>
        /// <param name="key">The Translation Key</param>
        /// <param name="def">The default for the translation</param>
        /// <param name="replaces">Any string.Formats you want to preform, do them here as default</param>
        /// <returns></returns>
        public Response Reply(string key, string def, params string[] replaces)
        {
            var trans = replaces != null && replaces?.Length > 0 ? 
                string.Format(Translate(key, def), replaces) : 
                Translate(key, def);
            return Reply(trans);
        }

        /// <summary>
        /// Replies to the message at hand with a string
        /// </summary>
        /// <param name="data">The Message to send</param>
        /// <returns>The response of the message</returns>
        public Response Reply(VoiceMessage data)
        {
            return Bot.Reply(Message, data).Result;
        }
        #endregion

        #region Private Message
        /// <summary>
        /// Sends a private messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(int id, string data)
        {
            return Bot.PrivateMessage(id, data).Result;
        }

        /// <summary>
        /// Sends a private messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(int id, Bitmap data)
        {
            return Bot.PrivateMessage(id, data).Result;
        }

        /// <summary>
        /// Sends a private messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(int id, byte[] data)
        {
            return Bot.PrivateMessage(id, data).Result;
        }

        /// <summary>
        /// Sends a private messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <param name="isWeb">Whether or not its a web request</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(int id, string data, bool isWeb)
        {
            return Bot.PrivateMessage(id, data, isWeb).Result;
        }

        /// <summary>
        /// Short cut to Reply with translations
        /// </summary>
        /// <param name="id">Id of the user to send to</param>
        /// <param name="key">The key of the translation</param>
        /// <param name="def">The default incase translation fails</param>
        /// <returns>The response of the message</returns>
        public Response PrivateMessage(int id, string key, string def)
        {
            var trans = Translate(key, def);
            return PrivateMessage(id, trans);
        }

        /// <summary>
        /// Replies to the current user in pm
        /// </summary>
        /// <param name="data">The message to send</param>
        /// <returns>The response to the message</returns>
        public Response PrivateMessage(string data)
        {
            return PrivateMessage(Message.SourceId, data);
        }

        /// <summary>
        /// Replies to the current user in pm with an image
        /// </summary>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(Bitmap data)
        {
            return Bot.PrivateMessage(Message.SourceId, data).Result;
        }

        /// <summary>
        /// Replies to the current user in pm with an image
        /// </summary>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(byte[] data)
        {
            return Bot.PrivateMessage(Message.SourceId, data).Result;
        }

        /// <summary>
        /// Replies to the current user in pm with an image from either the web or the computer
        /// </summary>
        /// <param name="data">The message to send</param>
        /// <param name="isWeb">Whether or not its a web request</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(string data, bool isWeb)
        {
            return Bot.PrivateMessage(Message.SourceId, data, isWeb).Result;
        }

        /// <summary>
        /// Short cut to Private Message with translations
        /// </summary>
        /// <param name="key">The key of the translation</param>
        /// <param name="def">The default incase translation fails</param>
        /// <returns>The response of the message</returns>
        public Response PrivateMessage(string key, string def)
        {
            var trans = Translate(key, def);
            return PrivateMessage(Message.SourceId, trans);
        }
        
        /// <summary>
        /// Replies to the current user in pm with an image
        /// </summary>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response PrivateMessage(VoiceMessage data)
        {
            return Bot.PrivateMessage(Message.SourceId, data).Result;
        }
        #endregion

        #region Group Message
        /// <summary>
        /// Short cut to sending a group message with translations
        /// </summary>
        /// <param name="id">Group to send to</param>
        /// <param name="key">The key of the translation</param>
        /// <param name="def">The default incase the translation doesnt exist</param>
        /// <returns>The response of the message</returns>
        public Response GroupMessage(int id, string key, string def)
        {
            var trans = Translate(key, def);
            return GroupMessage(id, trans);
        }

        /// <summary>
        /// Sends a Group messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response GroupMessage(int id, string data)
        {
            return Bot.GroupMessage(id, data).Result;
        }

        /// <summary>
        /// Sends a Group messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response GroupMessage(int id, Bitmap data)
        {
            return Bot.GroupMessage(id, data).Result;
        }

        /// <summary>
        /// Sends a Group messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response GroupMessage(int id, byte[] data)
        {
            return Bot.GroupMessage(id, data).Result;
        }

        /// <summary>
        /// Sends a Group messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <param name="isWeb">Whether or not its a web request</param>
        /// <returns>The response</returns>
        public Response GroupMessage(int id, string data, bool isWeb)
        {
            return Bot.GroupMessage(id, data, isWeb).Result;
        }

        /// <summary>
        /// Sends a Group messsage
        /// </summary>
        /// <param name="id">Id of the person to send to</param>
        /// <param name="data">The message to send</param>
        /// <returns>The response</returns>
        public Response GroupMessage(int id, VoiceMessage data)
        {
            return Bot.GroupMessage(id, data).Result;
        }

        #endregion

        #region Short cuts to actions

        /// <summary>
        /// Commits an admin action
        /// </summary>
        /// <param name="action">The action to commit</param>
        /// <param name="user">The User to commit against</param>
        /// <param name="group">The Group to commit in</param>
        /// <returns>The response</returns>
        public Response AdminAction(AdminActions action, UserId user, GroupId group)
        {
            return Bot.AdminAction(action, user, group).Result;
        }

        /// <summary>
        /// Joins the specified group (Optional=Group password)
        /// </summary>
        /// <param name="name">Name of the group to join</param>
        /// <param name="password">Password of the group to join (optional)</param>
        /// <returns>The response</returns>
        public Response JoinGroup(string name, string password = null)
        {
            return Bot.JoinGroup(name, password).Result;
        }

        /// <summary>
        /// Requests a users profile
        /// </summary>
        /// <param name="id">Id of the user to request</param>
        /// <returns>The users profile</returns>
        public User GetUser(UserId id)
        {
            return Bot.UserInfo(id).Result;
        }

        /// <summary>
        /// Gets the profiles for the collection of user ids
        /// </summary>
        /// <param name="ids">The users to fetch the profiles for</param>
        /// <returns>User profiles</returns>
        public User[] GetUsers(UserId[] ids)
        {
            return Bot.UsersInfo(ids).Result;
        }

        /// <summary>
        /// Gets the profiles for the collection of user ids
        /// </summary>
        /// <param name="ids">The users to fetch the profiles for</param>
        /// <returns>User profiles</returns>
        public User[] GetUsers(int[] ids)
        {
            return Bot.UsersInfo(ids).Result;
        }
        #endregion

        #region Message Fetchers

        /// <summary>
        /// Requests a message from the user.
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            return RequestContent().Result?.Content;
        }

        /// <summary>
        /// Request a number from the user
        /// </summary>
        /// <param name="num">The number requested</param>
        /// <returns>Whether the results are a number</returns>
        public bool GetInt(out int num)
        {
            var msg = GetMessage();
            return int.TryParse(msg, out num);
        }

        /// <summary>
        /// Request a number from the user
        /// </summary>
        /// <param name="num">The number requested</param>
        /// <returns>Whether the results are a number</returns>
        public bool GetDouble(out double num)
        {
            var msg = GetMessage();
            return double.TryParse(msg, out num);
        }

        /// <summary>
        /// Request a bool based on what your definition of yes is.
        /// </summary>
        /// <param name="yes">Your definition of yes, defaults to "y"</param>
        /// <returns>Whether the message matches or not</returns>
        public bool GetBool(string yes = "y")
        {
            return GetMessage().ToLower().Trim().StartsWith(yes.ToLower());
        }

        /// <summary>
        /// Request the next voice message to come down the line.
        /// </summary>
        /// <returns>Bytes of the voice message</returns>
        public byte[] GetVoice()
        {
            var msg = RequestContent(DataType.VoiceMessage).Result;

            return Static.PalringoEncoding.GetBytes(msg?.Content ?? "");
        }

        /// <summary>
        /// Request the next image message to come down the line
        /// </summary>
        /// <returns>Image Link</returns>
        public string GetImageLink()
        {
            return RequestContent(DataType.Image).Result?.Content;
        }

        /// <summary>
        /// Requests the next image message to come down the line
        /// </summary>
        /// <returns>The image</returns>
        public Bitmap GetImage()
        {
            var link = GetImageLink();
            return ImageService.BitmapFromBytes(new WebClient().DownloadData(link));
        }

        /// <summary>
        /// Request the next Rich Message to come down the line
        /// </summary>
        /// <returns>The Rich message data</returns>
        public string GetRichMessage()
        {
            return RequestContent(DataType.RichMessage).Result?.Content;
        }

        /// <summary>
        /// Request the next message that matches the enum type
        /// </summary>
        /// <typeparam name="T">Type of enum to send</typeparam>
        /// <param name="res">The result</param>
        /// <returns>Whether or not the message ended in an enum</returns>
        public bool GetEnum<T>(out T res)
        {
            var t = RequestContent().Result;
            if (t == null)
            {
                res = default(T);
                return false;
            }

            return t.Content.TryParseEnum(out res);
        }

        /// <summary>
        /// Gets the next message that matches a certain message type
        /// </summary>
        /// <param name="type">The type of message to match</param>
        /// <returns>The contents of the message</returns>
        public Message GetMessageMatch(DataType type)
        {
            return RequestContent(type).Result;
        }

        /// <summary>
        /// Cancels the current message request, setting the Message to null.
        /// </summary>
        public void CancelRequest()
        {
            Canceled = true;
            _tcs.TrySetResult(null);
        }

        private async Task<Message> RequestContent(DataType typeWatch = DataType.Text)
        {
            _tcs = new TaskCompletionSource<Message>();
            if (Message.MesgType == MessageType.Group)
                Bot.GetManager<ForumManager>().GroupInstances[Message.GroupId][Message.SourceUser] = (m) =>
                {
                    if (!typeWatch.HasFlag(m.ContentType) && m.ContentType != typeWatch)
                        return;
                    Message = m;
                    var res = _tcs.TrySetResult(m);
                };
            else
                Bot.GetManager<ForumManager>().PrivateInstances[Message.SourceUser] = (m) =>
                {
                    if (!typeWatch.HasFlag(m.ContentType) || m.ContentType != typeWatch)
                        return;
                    Message = m;
                    _tcs.TrySetResult(m);
                };
            return await _tcs.Task;
        }
        #endregion

        #region Forum Interactors

        /// <summary>
        /// Moves the current forum instance to the persons pm
        /// </summary>
        /// <returns></returns>
        public bool MoveToPrivate()
        {
            if (Message.MesgType == MessageType.Private)
                return false;

            if (Bot.GetManager<ForumManager>().PrivateInstances.ContainsKey(Message.SourceId))
                return false;

            var actResp = Bot.GetManager<ForumManager>().GroupInstances[Message.GroupId][Message.SourceId];
            Bot.GetManager<ForumManager>().RemoveInstance(Message);
            Bot.GetManager<ForumManager>().PrivateInstances.Add(Message.SourceUser, actResp);
            return true;
        }

        /// <summary>
        /// Moves the current Forum instance to a group
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public bool MoveToGroup(int groupid)
        {
            if (Message.MesgType == MessageType.Group)
                return false;

            if (!Bot.GetManager<ForumManager>().GroupInstances.ContainsKey(groupid))
                Bot.GetManager<ForumManager>().GroupInstances.Add(groupid, new System.Collections.Generic.Dictionary<int, System.Action<Message>>());

            if (Bot.GetManager<ForumManager>().GroupInstances[groupid].ContainsKey(Message.SourceId))
                return false;

            var actResp = Bot.GetManager<ForumManager>().PrivateInstances[Message.SourceUser];
            Bot.GetManager<ForumManager>().RemoveInstance(Message);
            Bot.GetManager<ForumManager>().GroupInstances[groupid].Add(Message.SourceUser, actResp);
            return true;
        }

        #endregion

        #region Translation

        /// <summary>
        /// Gets the requested key in the current language
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <param name="def">The default value incase there is no translation</param>
        /// <returns>The requested translation</returns>
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
        #endregion

        #endregion
    }
}
