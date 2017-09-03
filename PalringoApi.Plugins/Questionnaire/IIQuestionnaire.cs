using PalringoApi.Networking;
using PalringoApi.PacketData;
using System;

namespace PalringoApi.Plugins.Questionnaires
{
    /// <summary>
    /// Interface for IIQuestionnaire handling
    /// </summary>
    public interface IIQuestionnaire
    {
        /// <summary>
        /// The Bot Instances the questionnaire was started on.
        /// </summary>
        PalBot Bot { get; set; }

        /// <summary>
        /// Provides the language for the current instance.
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// The Message that was last sent.
        /// </summary>
        Message Message { get; set; }

        /// <summary>
        /// A custom holder for advanced users.
        /// </summary>
        object State { get; set; }

        /// <summary>
        /// The placeholder for the entrance to the questionnaire.
        /// </summary>
        /// <param name="message">The message that was sent minus the command</param>
        void Start(string message);

        /// <summary>
        /// The next action in line
        /// </summary>
        Action<Action<string>> Next { get; set; }

        /// <summary>
        /// What to do when finished.
        /// </summary>
        Action Finish { get; set; }

        /// <summary>
        /// Move the questionnaire to a private conversation (can fail if already in private conversation or the user already has a private questionnaire going).
        /// </summary>
        Func<bool> MoveToPrivate { get; set; }

        /// <summary>
        /// Move the questionnaire to a group conversation (can fail if already in a group conversation or the user already has a group questionnaire going).
        /// </summary>
        Func<GroupId, bool> MoveToGroup { get; set; }

        /// <summary>
        /// Contains an instance of the attribute <see cref="Plugins.Questionnaire"/> used to make instance of <see cref="IIQuestionnaire"/> 
        /// </summary>
        Questionnaire AttributeInstance { get; set; }

        /// <summary>
        /// Event that gets called upon the questionnaire being canceled.
        /// </summary>
        event DataCarrier OnCancel;

        /// <summary>
        /// Event that gets called upon the questionnaire being finished.
        /// </summary>
        event DataCarrier OnFinish;
    }
}
