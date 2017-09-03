using PalringoApi.Plugins;

namespace PalringoApi.Test.PluginsTest
{
    [Questionnaire("!quest survey", CancelWord = "cancel")]
    public class QuestionnaireTest : IQuestionnaire
    {
        public int Rating { get; set; }
        public string Suggestions { get; set; }

        public override void Start(string message)
        {
            Reply("From 1 to 5, how would you rate the bot? (1 being low, 5 being high)");
            Next(GetRating);
        }

        private void GetRating(string message)
        {
            int rating;
            if (int.TryParse(message, out rating))
            {
                Rating = rating;
                Reply("Do you have any suggestions?");
                Next(GetSuggestion);
                return;
            }

            Start(message);
        }

        private void GetSuggestion(string message)
        {
            Suggestions = message;
            Reply("Thanks! I'll pass along your rating and suggestion(s) to a bot admin!");
            foreach (var item in Bot.Settings.Admins)
            {
                PrivateMessage(item, $@"User: {Message.SourceUser.GetUser(Bot).Nickname} ({Message.SourceUser})
Rating: {Rating}
Suggestion(s): {Suggestions}");
            }
            Finish();
        }
    }
}
