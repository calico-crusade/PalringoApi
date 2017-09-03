using PalringoApi.Networking;
using PalringoApi.PacketData;
using PalringoApi.PluginManagement.Permissions;
using PalringoApi.Subprofile.Types;
using PalringoApi.Utilities;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using rs = System.Action<PalringoApi.PacketData.Response>;

namespace PalringoApi
{
    public partial class PalBot
    {
        #region Message Senders
        private void DoMessage(Message msg, rs rs)
        {
            msg.OnResponse = rs;
            if (msg.ContentType == DataType.Text || msg.ContentType == DataType.RichMessage)
                msg.Content = msg.Content.GetPalringoString();
            if (MessageQueue == null)
            {
                _client.WritePackets(msg.Package().Watch(this, rs));
                this.On.Trigger("e", new Exception("Message Queue is null! Sending without Throttle handling."));
                return;
            }
            MessageQueue.Drop(msg);
        }

#pragma warning disable CS0618 // Type or member is obsolete

        /// <summary>
        /// Reply to a message with a string
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <param name="onResponse"></param>
        public void Reply(Message msg, string data, rs onResponse = null)
        {
            DoMessage(new Message(msg.ReturnAddress, msg.MesgType, DataType.Text, data), onResponse);
        }

        /// <summary>
        /// Reply to a message with image bytes
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <param name="onResponse"></param>
        public void Reply(Message msg, byte[] data, rs onResponse = null)
        {
            DoMessage(new Message(msg.ReturnAddress,
                msg.MesgType, DataType.Image,
                ImageService.ConvertToJpg(data)), onResponse);
        }

        /// <summary>
        /// Reply to a message with a bitmap
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <param name="onResponse"></param>
        public void Reply(Message msg, Bitmap data, rs onResponse = null)
        {
            msg.MimeType = "image/jpeg";
            DoMessage(new Message(msg.ReturnAddress,
                msg.MesgType, DataType.Image,
                ImageService.BytesFromBitmap(ImageService.ConvertToJpg(data))), onResponse);
        }

        /// <summary>
        /// Reply to a message with an image from URL or File
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="uri"></param>
        /// <param name="isWeb"></param>
        /// <param name="onResponse"></param>
        public void Reply(Message msg, string uri, bool isWeb, rs onResponse = null)
        {
            try
            {
                byte[] data;
                if (isWeb)
                    data = new WebClient().DownloadData(uri);
                else
                    data = System.IO.File.ReadAllBytes(uri);
                
                Reply(msg, ImageService.BitmapFromBytes(data), onResponse);
            }
            catch
            {
                throw new System.Exception("Invalid image uri");
            }
        }
        
        /// <summary>
        /// Reply to a message with a voice message
        /// </summary>
        /// <param name="msg">The message to reply to</param>
        /// <param name="data">The voice message data</param>
        /// <param name="onResponse">The response to the message</param>
        public void Reply(Message msg, VoiceMessage data, rs onResponse = null)
        {
            msg.MimeType = "Audio/speex";
            DoMessage(new Message(msg.ReturnAddress,
                msg.MesgType, DataType.VoiceMessage,
                data), onResponse);
        }
        
        /// <summary>
        /// Reply to a message asynchronously with a string
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> Reply(Message msg, string data)
        {
            var tcs = new TaskCompletionSource<Response>();
            Reply(msg, data, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Reply to a message asynchronously with image bytes
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> Reply(Message msg, byte[] data)
        {
            var tcs = new TaskCompletionSource<Response>();
            Reply(msg, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Reply to a message asynchronously with a bitmap
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> Reply(Message msg, Bitmap data)
        {
            var tcs = new TaskCompletionSource<Response>();
            Reply(msg, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Reply to a message asynchronously with an image from URL or File
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="uri"></param>
        /// <param name="isWeb"></param>
        /// <returns></returns>
        public async Task<Response> Reply(Message msg, string uri, bool isWeb)
        {
            var tcs = new TaskCompletionSource<Response>();
            Reply(msg, uri, isWeb, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Reply to a message asynchronously with a voice message
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> Reply(Message msg, VoiceMessage data)
        {
            var tsc = new TaskCompletionSource<Response>();
            Reply(msg, data, (re) =>
            {
                try
                {
                    tsc.TrySetResult(re);
                }
                catch { }
            });
            return await tsc.Task;
        }

        /// <summary>
        /// Send a private message containing a string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void PrivateMessage(int id, string data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Private, DataType.Text, data), OnResponse);
        }

        /// <summary>
        /// Send a private message containing image bytes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void PrivateMessage(int id, byte[] data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Private, DataType.Image, ImageService.ConvertToJpg(data)), OnResponse);
        }

        /// <summary>
        /// Send a private message containing an bitmap
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void PrivateMessage(int id, Bitmap data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Private, DataType.Image, ImageService.BytesFromBitmap(ImageService.ConvertToJpg(data))), OnResponse);
        }

        /// <summary>
        /// Send a private message containing an image from the web or file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uri"></param>
        /// <param name="isWeb"></param>
        /// <param name="onResponse"></param>
        public void PrivateMessage(int id, string uri, bool isWeb, rs onResponse = null)
        {
            try
            {
                byte[] data;
                if (isWeb)
                    data = new WebClient().DownloadData(uri);
                else
                    data = System.IO.File.ReadAllBytes(uri);

                PrivateMessage(id, ImageService.BitmapFromBytes(data), onResponse);
            }
            catch
            {
                throw new System.Exception("Invalid image uri");
            }
        }

        /// <summary>
        /// Send a private message containing a voice message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void PrivateMessage(int id, VoiceMessage data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Private, DataType.VoiceMessage, data), OnResponse);
        }

        /// <summary>
        /// Send a private message asynchronously containing a string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> PrivateMessage(int id, string data)
        {
            var tcs = new TaskCompletionSource<Response>();
            PrivateMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a private message asynchronously containing a vocie message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> PrivateMessage(int id, VoiceMessage data)
        {
            var tcs = new TaskCompletionSource<Response>();
            PrivateMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a private message asynchronously containing image bytes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> PrivateMessage(int id, byte[] data)
        {
            var tcs = new TaskCompletionSource<Response>();
            PrivateMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a private message asynchronously containing a bitmap
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> PrivateMessage(int id, Bitmap data)
        {
            var tcs = new TaskCompletionSource<Response>();
            PrivateMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a private message asynchronously containing an image from URL or File
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="isWeb"></param>
        /// <returns></returns>
        public async Task<Response> PrivateMessage(int id, string data, bool isWeb)
        {
            var tcs = new TaskCompletionSource<Response>();
            PrivateMessage(id, data, isWeb, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a group message containing a string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void GroupMessage(int id, string data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Group, DataType.Text, data), OnResponse);
        }

        /// <summary>
        /// Send a group message containing a string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void GroupMessage(int id, VoiceMessage data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Group, DataType.VoiceMessage, data), OnResponse);
        }

        /// <summary>
        /// Send a group message containing image bytes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void GroupMessage(int id, byte[] data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Group, DataType.Text, data), OnResponse);
        }

        /// <summary>
        /// Send a group message containing a bitmap
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="OnResponse"></param>
        public void GroupMessage(int id, Bitmap data, rs OnResponse = null)
        {
            DoMessage(new Message(id, MessageType.Group, DataType.Image, ImageService.BytesFromBitmap(ImageService.ConvertToJpg(data))), OnResponse);
        }

        /// <summary>
        /// Send a group message containing an image from a URL or File
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uri"></param>
        /// <param name="isWeb"></param>
        /// <param name="onResponse"></param>
        public void GroupMessage(int id, string uri, bool isWeb, rs onResponse = null)
        {
            try
            {
                byte[] data;
                if (isWeb)
                    data = new WebClient().DownloadData(uri);
                else
                    data = System.IO.File.ReadAllBytes(uri);

                GroupMessage(id, ImageService.BitmapFromBytes(data), onResponse);
            }
            catch
            {
                throw new System.Exception("Invalid image uri");
            }
        }

        /// <summary>
        /// Send a group message asynchronously containing a string
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> GroupMessage(int id, string data)
        {
            var tcs = new TaskCompletionSource<Response>();
            GroupMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a group message asynchronously containing image bytes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> GroupMessage(int id, byte[] data)
        {
            var tcs = new TaskCompletionSource<Response>();
            GroupMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a group message asynchronously containing image bytes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> GroupMessage(int id, VoiceMessage data)
        {
            var tcs = new TaskCompletionSource<Response>();
            GroupMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a group message asynchronously containing a bitmap
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Response> GroupMessage(int id, Bitmap data)
        {
            var tcs = new TaskCompletionSource<Response>();
            GroupMessage(id, data, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Send a group message asynchronously containing an image from file or web
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="isWeb"></param>
        /// <returns></returns>
        public async Task<Response> GroupMessage(int id, string data, bool isWeb)
        {
            var tcs = new TaskCompletionSource<Response>();
            GroupMessage(id, data, isWeb, (re) =>
            {
                try
                {
                    tcs.TrySetResult(re);
                }
                catch { }
            });
            return await tcs.Task;
        }
        #endregion

        #region Group Manipulators
        /// <summary>
        /// Commit an admin action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="user"></param>
        /// <param name="group"></param>
        /// <param name="onResponse"></param>
        public void AdminAction(AdminActions action, UserId user, GroupId group, rs onResponse = null)
        {
            if (AuthorizationEngine.AuthorizedUsers.Contains(user) &&
                (action == AdminActions.Ban ||
                action == AdminActions.Kick ||
                action == AdminActions.Silence))
                return;
            var p = PacketTemplates.AdminAction(action, group, user).Watch(this, onResponse);
            _client.WritePacket(p);
        }
        
        /// <summary>
        /// Commit an admin action asynchronously
        /// </summary>
        /// <param name="action"></param>
        /// <param name="user"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<Response> AdminAction(AdminActions action, UserId user, GroupId group)
        {
            var tcs = new TaskCompletionSource<Response>();
            AdminAction(action, user, group, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Join a group
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <param name="onResponse"></param>
        public void JoinGroup(string name, string pass, rs onResponse = null)
        {
            _client.WritePacket(PacketTemplates.JoinGroup(name.GetPalringoString(), pass.GetPalringoString()).Watch(this, onResponse));
        }

        /// <summary>
        /// Join a group without a password
        /// </summary>
        /// <param name="name"></param>
        /// <param name="onResponse"></param>
        public void JoinGroup(string name, rs onResponse = null)
        {
            JoinGroup(name, null, onResponse);
        }
        
        /// <summary>
        /// Join a group asynchronously
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public async Task<Response> JoinGroup(string name, string pass)
        {
            var tcs = new TaskCompletionSource<Response>();
            JoinGroup(name, pass, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Join a group asynchronously with a password
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Response> JoinGroup(string name)
        {
            var tcs = new TaskCompletionSource<Response>();
            JoinGroup(name, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Leave a group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onResponse"></param>
        public void LeaveGroup(int id, rs onResponse = null)
        {
            _client.WritePacket(PacketTemplates.LeaveGroup(id).Watch(this, onResponse));
            On.Trigger("gl", ((GroupId)id).GetGroup(this));
        }

        /// <summary>
        /// Leave a group asynchronously
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response> LeaveGroup(int id)
        {
            var tcs = new TaskCompletionSource<Response>();
            LeaveGroup(id, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Disconnect from palringo
        /// </summary>
        public void Disconnect()
        {
            _client.WritePacket(PacketTemplates.Bye());
            _client.Disconnect();
        }

        /// <summary>
        /// Update group profile information
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="shortDescription"></param>
        /// <param name="longDescription"></param>
        /// <param name="language"></param>
        /// <param name="groupCategory"></param>
        /// <param name="onFinish"></param>
        public void UpdateGroupInfo(GroupId groupid, string shortDescription = null, string longDescription = null, Language? language = null, GroupCategory? groupCategory = null, rs onFinish = null)
        {
            string ps = $"action=palringo.group.update&" +
                $"group_id={groupid}&" +
                $"user_id={Information.Profile.Id}";

            if (!string.IsNullOrEmpty(shortDescription))
                ps += $"&short_description={HttpUtility.UrlEncode(shortDescription.GetPalringoString())}";
            if (!string.IsNullOrEmpty(longDescription))
                ps += $"&long_description={HttpUtility.UrlEncode(longDescription.GetPalringoString())}";
            if (language != null)
                ps += $"&language_id={(int)language.Value}";
            if (groupCategory != null)
                ps += $"&category_id={(int)groupCategory.Value}";


            _client.WritePacket(PacketTemplates.UrlQuery(ps, "api").Watch(this, onFinish));
        }

        /// <summary>
        /// Update group profile information asynchronously
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="shortDescription"></param>
        /// <param name="longDescription"></param>
        /// <param name="language"></param>
        /// <param name="groupCategory"></param>
        /// <returns></returns>
        public async Task<Response> UpdateGroupInfo(GroupId groupid, string shortDescription = null, string longDescription = null, Language? language = null, GroupCategory? groupCategory = null)
        {
            var tcs = new TaskCompletionSource<Response>();
            UpdateGroupInfo(groupid, shortDescription, longDescription, language, groupCategory, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }
        #endregion

        #region Profile Manipulators
        /// <summary>
        /// Update the bots avatar with a bitmap
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="onResponse"></param>
        public void UpdateAvatar(Bitmap avatar, rs onResponse = null)
        {
            _client.WritePackets(ImageService.UpdateAvatar(avatar).Watch(this, onResponse));
        }
        
        /// <summary>
        /// Update the bots avatar with image bytes
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="onResponse"></param>
        public void UpdateAvatar(byte[] avatar, rs onResponse = null)
        {
            _client.WritePackets(ImageService.UpdateAvatar(avatar).Watch(this, onResponse));
        } 

        /// <summary>
        /// Updates the extended profile of the user.
        /// </summary>
        /// <param name="onResp">What to do on the requests completion.</param>
        public void UpdateExtendedProfile(rs onResp = null)
        {
            _client.WritePackets(PacketChunkanizer.Chunk(PacketTemplates.SubProfileUpdate(this.Information.Profile)).Watch(this, onResp));   
        }

        /// <summary>
        /// Updates the basic profile of the user
        /// </summary>
        /// <param name="onResp">What to do on completion</param>
        public void UpdateBasicProfile(rs onResp = null)
        {
            _client.WritePackets(PacketChunkanizer.Chunk(PacketTemplates.UpdateInfo(this.Information.Profile.Nickname, this.Information.Profile.Status)).Watch(this, onResp));
        }

        /// <summary>
        /// Updates the basic profile of the user
        /// </summary>
        /// <param name="nickname">The nickname of the user</param>
        /// <param name="status">The status of the user</param>
        /// <param name="onResp">What to do on completion</param>
        public void UpdateBasicProfile(string nickname, string status, rs onResp = null)
        {
            this.Information.Profile.Nickname = nickname;
            this.Information.Profile.Status = status;
            _client.WritePackets(PacketChunkanizer.Chunk(PacketTemplates.UpdateInfo(this.Information.Profile.Nickname, this.Information.Profile.Status)).Watch(this, onResp));
        }

        /// <summary>
        /// Requests a user add you as a contact.
        /// </summary>
        /// <param name="userid">The person's id</param>
        /// <param name="onResp">What to do when it happens.</param>
        public void AddContact(UserId userid, rs onResp = null)
        {
            _client.WritePackets(PacketChunkanizer.Chunk(PacketTemplates.AddContact(userid)).Watch(this, onResp));
        }

        /// <summary>
        /// Update the bots avatar with a bitmap asynchronously
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public async Task<Response> UpdateAvatar(Bitmap avatar)
        {
            var tcs = new TaskCompletionSource<Response>();
            UpdateAvatar(avatar, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return await tcs.Task;
        }

        /// <summary>
        /// Update the bots avatar with image bytes asynchronously
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public async Task<Response> UpdateAvatar(byte[] avatar)
        {
            var tcs = new TaskCompletionSource<Response>();
            UpdateAvatar(avatar, (re) =>
            {
                tcs.TrySetResult(re);
            });
            return (await tcs.Task);
        }

        /// <summary>
        /// Asynchronously does a profile update of the extended profile.
        /// </summary>
        /// <returns>What happened when the profile was updated.</returns>
        public async Task<Response> UpdateExtendedProfile()
        {
            var tsc = new TaskCompletionSource<Response>();
            UpdateExtendedProfile((re) =>
            {
                tsc.TrySetResult(re);
            });
            return await tsc.Task;
        }

        /// <summary>
        /// Updates the basic profile of the user
        /// </summary>
        /// <returns>What happened when the request went through</returns>
        public async Task<Response> UpdateBasicProfile()
        {
            var tsc = new TaskCompletionSource<Response>();
            UpdateBasicProfile((r) =>
            {
                tsc.TrySetResult(r);
            });
            return await tsc.Task;
        }

        /// <summary>
        /// Updates the basic profile of the user
        /// </summary>
        /// <param name="nickname">The nickname of the user</param>
        /// <param name="status">The status of the user</param>
        /// <returns>What happened when the request went through</returns>
        public async Task<Response> UpdateBasicProfile(string nickname, string status)
        {
            var tsc = new TaskCompletionSource<Response>();
            UpdateBasicProfile(nickname, status, (r) =>
            {
                tsc.TrySetResult(r);
            });
            return await tsc.Task;
        }

        /// <summary>
        /// Requests a user add you as a contact.
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>The response.</returns>
        public async Task<Response> AddContact(UserId user)
        {
            var tsc = new TaskCompletionSource<Response>();
            AddContact(user, (r) =>
            {
                tsc.TrySetResult(r);
            });
            return await tsc.Task;
        }
        #endregion

        /// <summary>
        /// Writes a packet to the TCP stream
        /// [[EXPERIMENTAL! USE EXISTING PACKET WRITERS IF THEY EXIST]]
        /// </summary>
        /// <param name="packet">The packet to write</param>
        /// <param name="onResponse">The optional response</param>
        public void WritePacket(Packet packet, rs onResponse = null)
        {
            _client.WritePackets(PacketChunkanizer.Chunk(packet).Watch(this, onResponse));
        }
    }
}
