using PalringoApi.Networking;
using PalringoApi.PacketData;
using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalringoApi
{
    public partial class PalBot
    {
        /// <summary>
        /// Obtains user info from palringo
        /// </summary>
        /// <param name="id">The users ID to fetch</param>
        /// <param name="onFinish">What to do when you're finished</param>
        /// <param name="resp">What to do on the response packet</param>
        public void UserInfo(UserId id, Action<User> onFinish = null, Action<Response> resp = null)
        {
            var user = id.GetUser(this);
            if (user != null)
            {
                onFinish(user);
                return;
            }
            _client.WritePacket(PacketTemplates.Profile(id.ToString())
                .Watch(this, id, onFinish)
                .Watch(this, resp));
        }

        /// <summary>
        /// Obtains user info from pal in an async fashion
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The user info</returns>
        public async Task<User> UserInfo(UserId id)
        {
            var tcs = new TaskCompletionSource<User>();
            UserInfo(id, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Obtains user info from pal for multiple users in an async fashion
        /// </summary>
        /// <param name="ids">The ids of the users</param>
        /// <returns>The user information</returns>
        public async Task<User[]> UsersInfo(UserId[] ids)
        {
            var tcs = new TaskCompletionSource<User[]>();
            MultiUserSelect.GetUsers(this, ids.Select(t => (int)t).ToArray(), (r) =>
            {
                tcs.SetResult(r);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Obtains user info from pal for multiple users in an async fashion
        /// </summary>
        /// <param name="ids">The ides of the users</param>
        /// <returns>The user information</returns>
        public async Task<User[]> UsersInfo(int[] ids)
        {
            var tcs = new TaskCompletionSource<User[]>();
            MultiUserSelect.GetUsers(this, ids, (r) =>
            {
                tcs.SetResult(r);
            });
            return await tcs.Task;
        }

        private int OnNextMessagesCount = 0;
        private Dictionary<int, MessageResponseHandling> OnNextMessages = new Dictionary<int, MessageResponseHandling>();

        private class MessageResponseHandling
        {
            public Func<Message, bool> Match { get; set; }
            public Action<Message> Finish { get; set; }

            public MessageResponseHandling(Func<Message, bool> m, Action<Message> f)
            {
                Match = m;
                Finish = f;
            }
        }

        private void MessageRecieved(Message msg)
        {
            var methods = OnNextMessages.Where(t => t.Value.Match(msg)).ToArray();
            foreach (var m in methods)
                m.Value.Finish(msg);
        }

        /// <summary>
        /// Obtains the next message sent
        /// </summary>
        /// <param name="matches">check to see if the message matches the data</param>
        /// <returns>Obtains the next message</returns>
        public Task<Message> NextMessage(Func<Message, bool> matches)
        {
            var tcs = new TaskCompletionSource<Message>();

            var id = OnNextMessagesCount;

            OnNextMessagesCount += 1;

            OnNextMessages.Add(id, new MessageResponseHandling(matches, (m) =>
            {
                OnNextMessages.Remove(id);
                tcs.TrySetResult(m);
            }));

            return tcs.Task;
        }

        #region Private Message Async Tasks

        /// <summary>
        /// Obtains the next private message recieved
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextPrivateMessage()
        {
            return NextMessage((m) => m.MesgType == MessageType.Private &&
                m.ContentType == DataType.Text);
        }

        /// <summary>
        /// Obtains the next private message recieved from a certain user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextPrivateMessage(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Private && 
                m.SourceUser == id && 
                m.ContentType == DataType.Text);
        }

        /// <summary>
        /// Obtains the next private image
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextPrivateImage()
        {
            return NextMessage((m) => m.MesgType == MessageType.Private && 
                m.ContentType == DataType.Image);
        }

        /// <summary>
        /// Obtains the next private image from a certain user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextPrivateImage(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Private &&
                m.ContentType == DataType.Image &&
                m.SourceUser == id);
        }


        /// <summary>
        /// Obtains the next rich private message
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextPrivateRich()
        {
            return NextMessage((m) => m.MesgType == MessageType.Private &&
                m.ContentType == DataType.RichMessage);
        }

        /// <summary>
        /// obtains the next rich private message from a certain user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextPrivateRich(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Private &&
                m.ContentType == DataType.RichMessage &&
                m.SourceUser == id);
        }


        /// <summary>
        /// Obtains the next voice private message
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextPrivateVoice()
        {
            return NextMessage((m) => m.MesgType == MessageType.Private &&
                m.ContentType == DataType.VoiceMessage);
        }

        /// <summary>
        /// Obtains the next voice private message from a certain group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextPrivateVoice(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Private &&
                m.ContentType == DataType.VoiceMessage &&
                m.SourceUser == id);
        }
        #endregion

        #region Group Message Async Tasks
        /// <summary>
        /// Gets the next group message
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextGroupMessage()
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Text);
        }
         
        /// <summary>
        /// Gets the next group message said in a certain group
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        public Task<Message> NextGroupMessage(GroupId grp)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Text &&
                m.GroupId == grp);
        }

        /// <summary>
        /// Gets the next group message said by a certain user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupMessage(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Text &&
                m.SourceUser == id);
        }

        /// <summary>
        /// Gets the next group message said in a certain group by a certain user
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupMessage(GroupId grp, UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Text &&
                m.GroupId == grp &&
                m.SourceUser == id);
        }

        /// <summary>
        /// Gets the next group image message
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextGroupImage()
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Image);
        }

        /// <summary>
        /// Gets the next group image message said in a certain group
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        public Task<Message> NextGroupImage(GroupId grp)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Image &&
                m.GroupId == grp);
        }

        /// <summary>
        /// Gets the next group image message said by a certain user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupImage(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Image &&
                m.SourceUser == id);
        }

        /// <summary>
        /// Gets the next group image message said in a certain group by a certain user
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupImage(GroupId grp, UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.Image &&
                m.GroupId == grp &&
                m.SourceUser == id);
        }

        /// <summary>
        /// Gets the next group voice message
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextGroupVoice()
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.VoiceMessage);
        }

        /// <summary>
        /// Gets the next group voice message said in a certain group
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        public Task<Message> NextGroupVoice(GroupId grp)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.VoiceMessage &&
                m.GroupId == grp);
        }

        /// <summary>
        /// Gets the next group voice message said by a certain user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupVoice(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.VoiceMessage &&
                m.SourceUser == id);
        }

        /// <summary>
        /// Gets the next group voice message said in a certain group by a certain user
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupVoice(GroupId grp, UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.VoiceMessage &&
                m.GroupId == grp &&
                m.SourceUser == id);
        }

        /// <summary>
        /// Gets the next group rich message
        /// </summary>
        /// <returns></returns>
        public Task<Message> NextGroupRich()
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.RichMessage);
        }

        /// <summary>
        /// Gets the next group rich message said in a certain group
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        public Task<Message> NextGroupRich(GroupId grp)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.RichMessage &&
                m.GroupId == grp);
        }

        /// <summary>
        /// Gets the next group rich message said by a certain user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupRich(UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.RichMessage &&
                m.SourceUser == id);
        }

        /// <summary>
        /// Gets the next group rich message said in a certain group by a certain user
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Message> NextGroupRich(GroupId grp, UserId id)
        {
            return NextMessage((m) => m.MesgType == MessageType.Group &&
                m.ContentType == DataType.RichMessage &&
                m.GroupId == grp &&
                m.SourceUser == id);
        }
        #endregion
    }
}
