using PalringoApi.Networking;
using PalringoApi.PacketData;
using PalringoApi.PluginManagement;
using PalringoApi.PluginManagement.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PalringoApi.Plugins.Questionnaires
{
    /// <summary>
    /// Manager for Questionnaire plugins
    /// </summary>
    public class QuestionnaireManager : IManager
    {
        /// <summary>
        /// Called when a questionnaire is canceled (for internal use)
        /// </summary>
        public event DataCarrier<IIQuestionnaire> QuestionnaireCanceled = delegate { };
        /// <summary>
        /// Called when a questionnaire is finished (for internal use)
        /// </summary>
        public event DataCarrier<IIQuestionnaire> QuestionnaireFinished = delegate { };

        /// <summary>
        /// Storage for all active group instances of the Questionnaire plugins
        /// </summary>
        private Dictionary<int, Dictionary<int, KeyValuePair<IIQuestionnaire, Action<string>>>> GroupInstances { get; set; } = new Dictionary<int, Dictionary<int, KeyValuePair<IIQuestionnaire, Action<string>>>>();

        /// <summary>
        /// Storage for all active private instances of the Questionnaire plugins
        /// </summary>
        private Dictionary<int, KeyValuePair<IIQuestionnaire, Action<string>>> PrivateInstances { get; set; } = new Dictionary<int, KeyValuePair<IIQuestionnaire, Action<string>>>();
        
        private List<TypeInstance<Questionnaire>> Plugs { get; set; } = new List<TypeInstance<Questionnaire>>();

        /// <summary>
        /// Initial load of all questionnaire plugins
        /// </summary>
        /// <param name="bot"></param>
        public void Load(PalBot bot)
        {
            bot.On.GroupMessage += (m) => OnMessage(bot, m);
            bot.On.PrivateMessage += (m) => OnMessage(bot, m);

            var ps = typeof(IIQuestionnaire).GetAllTypes()
                .Where(t => Attribute.IsDefined(t, typeof(Questionnaire)))
                .Select(t => new TypeInstance<Questionnaire>(t, t.GetCustomAttribute<Questionnaire>())).ToList();

            foreach(var plug in ps)
            {
                if (bot.Translations != null)
                {
                    foreach (var tr in bot.Translations[plug.Value.TranslationKey])
                    {
                        var c = new TypeInstance<Questionnaire>(plug.Type, plug.Value.Clone(tr.Value));
                        c.Language = tr.Language;
                        Plugs.Add(c);
                    }
                }

                Plugs.Add(plug);
            }
        }

        public bool IsInGroupQuestionnaire(int userid)
        {
            return GroupInstances.Any(t => t.Value.ContainsKey(userid));
        }

        public bool IsInGroupQuestionnaire(int userid, out int[] groupids)
        {
            var items = GroupInstances.Where(t => t.Value.ContainsKey(userid)).ToArray();

            groupids = items.Select(t => t.Key).ToArray();

            return groupids.Length > 0;
        }

        public bool IsInGroupQuestionnaire(int groupid, int userid)
        {
            return GroupInstances.ContainsKey(groupid) ? GroupInstances[groupid].ContainsKey(userid) : false;
        }

        public bool IsInPrivateQuestionnaire(int userid)
        {
            return PrivateInstances.ContainsKey(userid);
        }

        public bool RemovePrivateQuestionnaire(int userid)
        {
            if (!PrivateInstances.ContainsKey(userid))
                return false;

            PrivateInstances[userid].Key.Finish();
            return true;
        }

        public bool RemoveGroupQuestionnaire(int userid)
        {
            int[] groupids;
            if (!IsInGroupQuestionnaire(userid, out groupids))
                return false;

            foreach(var item in groupids)
            {
                GroupInstances[item][userid].Key.Finish();
            }
            return true;
        }

        public bool RemoveGroupQuestionnaire(int groupid, int userid)
        {
            if (!GroupInstances.ContainsKey(groupid))
                return false;
            if (!GroupInstances[groupid].ContainsKey(userid))
                return false;
            GroupInstances[groupid][userid].Key.Finish();
            return true;
        }

        private void OnMessage(PalBot bot, Message message)
        {
            try
            {
                if (message.MesgType == Subprofile.Types.MessageType.Group && GroupInstances.ContainsKey(message.GroupId) && GroupInstances[message.GroupId].ContainsKey(message.SourceId))
                {
                    var p = GroupInstances[message.GroupId][message.SourceId];
                    RunQuestionnaire(p, message, bot);
                    return;
                }
                if (message.MesgType == Subprofile.Types.MessageType.Private && PrivateInstances.ContainsKey(message.SourceId))
                {
                    var p = PrivateInstances[message.SourceId];
                    RunQuestionnaire(p, message, bot);
                    return;
                }
            }
            catch (Exception ex)
            {
                bot.On.Trigger("e", ex);
                return;
            }

            DoTyping(bot, message, null, message.Content, null);
        }

        private void RunQuestionnaire(KeyValuePair<IIQuestionnaire, Action<string>> p, Message message, PalBot bot)
        {
            p.Key.Message = message;
            p.Key.Bot = bot;
            if (!string.IsNullOrEmpty(p.Key.AttributeInstance.CancelWord) && message.Content.ToLower().Trim() == p.Key.AttributeInstance.CancelWord)
            {
                QuestionnaireCanceled(p.Key);
                p.Key.Finish();
                return;
            }
            p.Value(message.Content);
        }

        private void AddQuestionnaire(IIQuestionnaire quest, Action<string> method, int groupid, int userid, PalBot bot)
        {
            if (!GroupInstances.ContainsKey(groupid))
                GroupInstances.Add(groupid, new Dictionary<int, KeyValuePair<IIQuestionnaire, Action<string>>>());
            GroupInstances[groupid].Add(userid, new KeyValuePair<IIQuestionnaire, Action<string>>(quest, method));

            quest.Next = (a) => GroupInstances[groupid][userid] = new KeyValuePair<IIQuestionnaire, Action<string>>(quest, a);
            quest.Finish = () =>
            {
                QuestionnaireFinished(quest);
                GroupInstances[groupid].Remove(userid);
            };
            quest.MoveToGroup = (i) => false;
            quest.MoveToPrivate = () => 
            {
                if (PrivateInstances.ContainsKey(userid))
                    return false;
                
                AddQuestionnaire(quest, GroupInstances[groupid][userid].Value, userid, bot);
                GroupInstances[groupid].Remove(userid);
                return true;
            };
            ((IQuestionnaire)quest)._doStartUp(bot);
        }

        private void AddQuestionnaire(IIQuestionnaire quest, Action<string> method, int userid, PalBot bot)
        {
            PrivateInstances.Add(userid, new KeyValuePair<IIQuestionnaire, Action<string>>(quest, method));

            quest.Next = (a) => PrivateInstances[userid] = new KeyValuePair<IIQuestionnaire, Action<string>>(quest, a);
            quest.Finish = () =>
            {
                QuestionnaireFinished(quest);
                PrivateInstances.Remove(userid);
            };
            quest.MoveToPrivate = () => false;
            quest.MoveToGroup = (i) =>
            {
                if (!GroupInstances.ContainsKey(i))
                    GroupInstances.Add(i, new Dictionary<int, KeyValuePair<IIQuestionnaire, Action<string>>>());
                if (GroupInstances[i].ContainsKey(userid))
                    return false;
                AddQuestionnaire(quest, PrivateInstances[userid].Value, i, userid, bot);
                PrivateInstances.Remove(userid);
                return true;
            };
            ((IQuestionnaire)quest)._doStartUp(bot);
        }

        private void DoTyping(PalBot bot, Message message, object state, string descriptor, Func<string, string> remover)
        {
            var group = message.GroupId.GetGroup(bot);
            var user = message.SourceUser.GetUser(bot);

            string msg = descriptor.Trim();
            foreach(var p in Plugs)
            {
                var col = p.Value;
                if (!col.Type.HasFlag(message.MesgType) ||
                    (bot.Title != null && col.For != null && col.For.Length > 0 && !col.For.Contains(bot.Title)) ||
                    !msg.ToLower().StartsWith(col.Trigger))
                    continue;

                if (!col.PermissionEngine.CheckPermissions(col.Permissions, user, group))
                {
                    bot.On.Trigger("pf", bot, new FailedPermissionsReport(message, col.Permissions, col.Trigger, PluginType.Questionnaire));
                    continue;
                }

                var submsg = remover == null ? msg.Remove(0, col.Trigger.Length).Trim() : remover(message.Content);

                var i = (IIQuestionnaire)Activator.CreateInstance(p.Type);
                i.AttributeInstance = p.Value;
                if (message.MesgType == Subprofile.Types.MessageType.Group)
                    AddQuestionnaire(i, i.Start, message.GroupId, message.SourceId, bot);
                else
                    AddQuestionnaire(i, i.Start, message.SourceId, bot);
                try
                {
                    i.Language = p.Language;
                    i.Message = message;
                    i.Bot = bot;
                    i.State = state;
                    i.Start(submsg);
                }
                catch (Exception ex)
                {
                    bot.On.Trigger("e", ex);
                }
            }
        }
    }
}
