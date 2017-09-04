using PalringoApi.Subprofile.Types;
using System;
using System.Collections.Generic;

namespace PalringoApi.Utilities
{
    public static class TagDisector
    {
        public static void DoTags(Dictionary<string, int> nameToTagExamples)
        {
            foreach (var item in nameToTagExamples)
            {
                DisplayTags(item.Key, item.Value);
            }
        }

        private static void DisplayTags(string name, int tagNum)
        {
            Console.WriteLine(name);
            var tag = (Tags)tagNum;
            foreach (var t in tag.AllFlags())
            {
                if (tag.HasFlag(t))
                {
                    Console.WriteLine(t.ToString() + " - " + (int)t);
                    tag = tag & ~t;
                }
            }
            Console.WriteLine("Left (num): " + (int)tag);
            Console.WriteLine("Left (bin): " + Convert.ToString((int)tag, 2));
            Console.WriteLine("\r\n\r\n");
        }
    }
}
