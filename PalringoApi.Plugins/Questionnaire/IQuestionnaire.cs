using System;
using Newtonsoft.Json;
using PalringoApi.Networking;
using PalringoApi.PluginManagement;
using PalringoApi.Plugins.Questionnaires;

namespace PalringoApi.Plugins
{
    /// <summary>
    /// Inheritable instance of <see cref="IIQuestionnaire"/> that has helper methods on it.
    /// </summary>
    public abstract class IQuestionnaire : SendersInstance, IIQuestionnaire
    {
        /// <summary>
        /// Closes the questionnaire and dumps resources
        /// </summary>
        [JsonIgnore]
        public Action Finish { get; set; }
        /// <summary>
        /// A state for advanced users of the system.
        /// </summary>
        [JsonIgnore]
        public object State { get; set; }
        /// <summary>
        /// Moves the questionnaire to a group conversation
        /// </summary>
        [JsonIgnore]
        public Func<GroupId, bool> MoveToGroup { get; set; }
        /// <summary>
        /// Moves the questionnaire to a private conversation
        /// </summary>
        [JsonIgnore]
        public Func<bool> MoveToPrivate { get; set; }
        /// <summary>
        /// Dictates the next method to be run when this one is completed.
        /// </summary>
        [JsonIgnore]
        public Action<Action<string>> Next { get; set; }

        /// <summary>
        /// Holder for the instance of the <see cref="Questionnaire"/> that created the current <see cref="IQuestionnaire"/> 
        /// </summary>
        [JsonIgnore]
        public Questionnaire AttributeInstance { get; set; }

        /// <summary>
        /// Provides the holder for the language the questionnaire is using.
        /// </summary>
        [JsonIgnore]
        public override string Language { get; set; }

        /// <summary>
        /// Called upon cancelation of the questionnaire
        /// </summary>
        public event DataCarrier OnCancel = delegate { };

        /// <summary>
        /// Called upon finishing of the questionnaire
        /// </summary>
        public event DataCarrier OnFinish = delegate { };

        /// <summary>
        /// The initial start method
        /// </summary>
        /// <param name="message"></param>
        public abstract void Start(string message);


        private bool DoneStartUp = false;
        /// <summary>
        /// Does the strart up handling for Questionnaires for the QuestionnaireCanceled and QuestionniareFinished delegates
        /// </summary>
        /// <param name="bot"></param>
        public void _doStartUp(PalBot bot)
        {
            if (!DoneStartUp)
            {
                bot.GetManager<QuestionnaireManager>().QuestionnaireCanceled += (i) =>
                {
                    if (i == this)
                        OnCancel();
                };
                bot.GetManager<QuestionnaireManager>().QuestionnaireFinished += (i) =>
                {
                    if (i == this)
                        OnFinish();
                };
                DoneStartUp = true;
            }

        }
    }
}
