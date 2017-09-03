using System;

namespace PalringoApi.Test
{
    using Utilities;
    public static class Program
    {
        public static PalBot Bot { get; set; }

        public static void Main(string[] args)
        {
            var settings = Settings.FromFile("bot.settings.json");
            Bot = new PalBot("TestBot", true);

            Bot.On.LoginSuccess += () => LoginSuccess();
            Bot.On.LoginFailed += (reason) => Console.WriteLine("Palringo bot not logged in: " + reason);

            Bot.Login(settings);

            Console.ReadKey();
        }

        private static void LoginSuccess()
        {
            Console.WriteLine("Palringo bot started and logged in!");
        }
    }
}
