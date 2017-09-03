namespace PalringoApi.Test.PluginsTest
{
    using PalringoApi.Plugins;

    public class ForumTest : IForum
    {
        [Forum("!forum survey", Permissions = Permission.All)]
        public void TestSurvey(string message)
        {
            int rating = 0;
            while (rating < 1 && rating > 5)
            {
                Reply("From 1 to 5, how would you rate the bot? (1 being low, 5 being high)!");
                GetInt(out rating);
            }

            string suggestion = null;

            while (suggestion == null || suggestion.Length <= 0)
            {
                Reply("Do you have any suggestions?");
                suggestion = GetMessage();
            }

            Reply("Thanks! I'll pass along your rating and suggestion(s) to a bot admin!");
            foreach(var item in Bot.Settings.Admins)
            {
                PrivateMessage(item, $@"User: {Message.SourceUser.GetUser(Bot).Nickname} ({Message.SourceUser})
Rating: {rating}
Suggestion(s): {suggestion}");
            }
        }
    }
}
