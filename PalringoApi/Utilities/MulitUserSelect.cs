using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PalringoApi
{
    /// <summary>
    /// Allows for the bot to grab multiple user profiles and return them all at once
    /// </summary>
    public static class MultiUserSelect
    {
        private static int Count = 0;
        private static Dictionary<int, Request> Requests = new Dictionary<int, Request>();

        private class Request
        {
            public int[] RequestedIds { get; set; }

            public Dictionary<int, User> FetchedUsers { get; set; }

            public List<int> FailedIds { get; set; }

            public Action<User[]> OnFinished { get; set; }

            public bool Finished => FetchedUsers.Count + FailedIds.Count == RequestedIds.Length;

            public Request(int[] ids, Action<User[]> onfin)
            {
                RequestedIds = ids;
                FetchedUsers = new Dictionary<int, User>();
                FailedIds = new List<int>();
                OnFinished = onfin;
            }
        }

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="ids"></param>
        /// <param name="onFinish"></param>
        public static void GetUsers(this PalBot bot, int[] ids, Action<User[]> onFinish)
        {
            var id = Count;
            Count += 1;

            var request = new Request(ids, onFinish);
            Requests.Add(id, request);

            foreach (var uid in ids)
            {
                bot.UserInfo(uid, (u) =>
                {
                    Requests[id].FetchedUsers.Add(uid, u);
                    if (Requests[id].Finished)
                    {
                        onFinish(Requests[id].FetchedUsers.Values.ToArray());
                        Requests.Remove(id);
                    }
                }, (r) =>
                {
                    if (r.Type == Subprofile.Types.Type.Code && r.Code == Code.NO_SUCH_WHATEVER)
                    {
                        Requests[id].FailedIds.Add(uid);
                        if (Requests[id].Finished)
                        {
                            onFinish(Requests[id].FetchedUsers.Values.ToArray());
                            Requests.Remove(id);
                        }
                    }
                });
            }
        }
    }
}
