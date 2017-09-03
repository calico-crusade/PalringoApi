using PalringoApi.Networking;
using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using rs = System.Action<PalringoApi.PacketData.Response>;

namespace PalringoApi
{
    /// <summary>
    /// A Collection of Extensions and helpful methods
    /// </summary>
    public static class Static
    {
        /// <summary>
        /// The Encoding Palringo uses for all communication
        /// </summary>
        public static Encoding PalringoEncoding => Encoding.Default;
        /// <summary>
        /// The encoding Windows uses for displaying unicode strings.
        /// </summary>
        public static Encoding DisplayEncoding => Encoding.UTF8;

        /// <summary>
        /// Joins two like collections
        /// </summary>
        /// <typeparam name="T">The type of data to join</typeparam>
        /// <param name="data">The first collection</param>
        /// <param name="objs">The second collection</param>
        /// <returns>The conjoined collection</returns>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> data, params T[] objs)
        {
            foreach (var item in data)
                yield return item;
            foreach (var item in objs)
                yield return item;
        }

        /// <summary>
        /// Splits a string into chunks of strings
        /// </summary>
        /// <param name="str">The string to split</param>
        /// <param name="chunkSize">The max length of a chunk</param>
        /// <returns>The chunked strings</returns>
        public static IEnumerable<string> SplitChunks(this string str, int chunkSize)
        {
            for (var i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }

        /// <summary>
        /// Splits a collection of data into chunks
        /// </summary>
        /// <typeparam name="T">The type of data to split</typeparam>
        /// <param name="inData">The data to split</param>
        /// <param name="chunkSize">The max length of a chunk</param>
        /// <returns>The chunked data</returns>
        public static IEnumerable<T[]> SplitChunks<T>(this IEnumerable<T> inData, int chunkSize)
        {
            var data = inData.ToArray();
            var chunk = new List<T>();
            for(var i = 0; i < data.Length; i++)
            {
                if (chunk.Count == chunkSize)
                {
                    yield return chunk.ToArray();
                    chunk = new List<T>();
                }

                chunk.Add(data[i]);
            }
            if (chunk.Count > 0)
                yield return chunk.ToArray();
        }

        /// <summary>
        /// Safely removes a chunk from a string ignoring out of bounds errors
        /// </summary>
        /// <param name="str">The string to remove from</param>
        /// <param name="start">The start index to remove from</param>
        /// <param name="length">How long the chunk to remove is</param>
        /// <returns>The data without the specified chunk</returns>
        public static string SafeRemove(this string str, int start, int length)
        {
            string o = "";
            for(var i = 0; i < str.Length; i++)
            {
                if (i >= start && i < start + length)
                    continue;
                o += str[i];
            }
            return o;
        }

        /// <summary>
        /// Watch a packet collection for the responses.
        /// </summary>
        /// <param name="packets">The packets to send</param>
        /// <param name="bot">The bot to send them on</param>
        /// <param name="onResponse">The response handler</param>
        /// <param name="onlyLast">Only watch for the last response in the cluge</param>
        /// <returns>The original array of packets.</returns>
        public static Packet[] Watch(this Packet[] packets, PalBot bot, rs onResponse, bool onlyLast = true)
        {
            try
            {
                if (!onlyLast)
                    foreach (var p in packets)
                        p.Watch(bot, onResponse);
                else
                    packets.Last().Watch(bot, onResponse);

            }
            catch (Exception ex)
            {
                bot.On.Trigger("e", ex);
            }
            return packets;
        }

        /// <summary>
        /// Returns a string converted from <see cref="PalringoEncoding"/> to <see cref="DisplayEncoding"/>
        /// </summary>
        /// <param name="palstr">The string to convert</param>
        /// <returns>The converted string</returns>
        public static string GetDisplayString(this string palstr)
        {
            return DisplayEncoding.GetString(PalringoEncoding.GetBytes(palstr ?? ""));
        }

        /// <summary>
        /// Returns a string converted from <see cref="DisplayEncoding"/> to <see cref="PalringoEncoding"/>
        /// </summary>
        /// <param name="disstr">The string to convert</param>
        /// <returns>The converted string</returns>
        public static string GetPalringoString(this string disstr)
        {
            return PalringoEncoding.GetString(DisplayEncoding.GetBytes(disstr ?? ""));
        }

        /// <summary>
        /// Converts a string from Windows1252 to UTF8
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>The converted string</returns>
        public static string Win1252ToUtf8(this string str)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(str ?? ""));
        }

        /// <summary>
        /// Converts a string from UTF8 to Windows1252
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>The converted string</returns>
        public static string Utf8ToWin1252(this string str)
        {
            return Encoding.Default.GetString(Encoding.UTF8.GetBytes(str ?? ""));
        }

        /// <summary>
        /// Gets the palringo device code from the specific device type
        /// </summary>
        /// <param name="type">The device type</param>
        /// <returns>The palringo device code.</returns>
        public static string GetStrDevice(this DeviceType type)
        {
            switch (type)
            {
                ///////////Device //////////what you type
                case DeviceType.Android: return "android";
                case DeviceType.PC: return "Windows x86";
                case DeviceType.Mac: return "Apple/Intel";
                case DeviceType.iPad: return "Apple/iPad/Premium";
                case DeviceType.iPhone: return "Apple/iPhone/Premium";
                case DeviceType.WindowsPhone7: return "Win/P7";
                case DeviceType.Web: return "WEB";
                default:
                case DeviceType.Generic: return "Java";
            }
        }

        /// <summary>
        /// Gets the palringo mimetype from the specified DataType
        /// </summary>
        /// <param name="type">The datatype</param>
        /// <returns>The palringo mimetype</returns>
        public static string FromDataType(this DataType type)
        {
            switch (type)
            {
                case DataType.Image: return "image/jpeg";
                case DataType.Text: return "text/plain";
                case DataType.VoiceMessage: return "audio/x-speex";
                default: return "text/html";
            }
        }

        /// <summary>
        /// Converts the palringo mimetype to the specified DataType
        /// </summary>
        /// <param name="type">The mimetype</param>
        /// <returns>The datatype</returns>
        public static DataType FromMimeType(this string type)
        {
            switch (type)
            {
                case "text/plain":
                    return DataType.Text;
                case "image/jpeg":
                case "text/image_link":
                    return DataType.Image;
                case "audio/x-speex":
                    return DataType.VoiceMessage;
                default:
                    return DataType.RichMessage;
            }
        }

        /// <summary>
        /// Converts the role from the adminaction
        /// </summary>
        /// <param name="action">The admin action</param>
        /// <returns>The role</returns>
        public static Role ToRole(this AdminActions action)
        {
            switch (action)
            {
                case AdminActions.Admin:
                    return Role.Admin;
                case AdminActions.Ban:
                    return Role.Banned;
                case AdminActions.Kick:
                    return Role.NotMember;
                case AdminActions.Mod:
                    return Role.Mod;
                case AdminActions.Silence:
                    return Role.Silenced;
                default:
                    return Role.User;
            }
        }

        /// <summary>
        /// Gets the allowed action of a role
        /// </summary>
        /// <param name="role">The role</param>
        /// <returns>The allowed admin actions</returns>
        public static AdminActions[] AllowedActions(this Role role)
        {
            switch (role)
            {
                case Role.Owner:
                case Role.Admin:
                    return new[] { AdminActions.Admin, AdminActions.Ban, AdminActions.Kick, AdminActions.Mod, AdminActions.Reset, AdminActions.Silence };
                case Role.Mod:
                    return new[] { AdminActions.Ban, AdminActions.Kick, AdminActions.Reset, AdminActions.Silence };
                default:
                    return new AdminActions[0];
            }
        }

        /// <summary>
        /// Writes a JSON Serialized object out to the console (for Debugging)
        /// </summary>
        /// <typeparam name="T">The object to write</typeparam>
        /// <param name="data">The data to write</param>
        public static void WriteObject<T>(this T data)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented));
        }

        /// <summary>
        /// Gets all of the posible Enum Flags from a sepific enum
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="enumFlag">The enum to get the flags of</param>
        /// <returns>A collection of all the flags in the enum</returns>
        public static T[] AllFlags<T>(this T enumFlag) where T: IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("enumFlag must be an Enum type");

            return Enum.GetValues(typeof(T)).Cast<T>().OrderBy(t => t.ToInt32(null)).ToArray();
        }

        /// <summary>
        /// Searches through the current appdomain and fetches all class instances of a certain type.
        /// </summary>
        /// <param name="type">The type to get</param>
        /// <param name="excludeAbstracts">Whether to include abstract classes or not</param>
        /// <param name="excludeInterfaces">Whether to include interfaces or not</param>
        /// <returns>A collection of all the classes of a certain type</returns>
        public static IEnumerable<System.Type> GetAllTypes(this System.Type type, bool excludeAbstracts = true, bool excludeInterfaces = true)
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblies = assembly.GetReferencedAssemblies();

            foreach(var asm in assemblies.Union(assembly.GetName()))
            {
                var asml = Assembly.Load(asm);
                foreach (var ty in asml.DefinedTypes)
                {
                    if ((excludeAbstracts && ty.IsAbstract) || 
                        (excludeInterfaces && ty.IsInterface) ||
                        !type.IsAssignableFrom(ty))
                        continue;
                    yield return ty;
                }
            }
        }
        
        /// <summary>
        /// Parses an Enum from a string
        /// </summary>
        /// <typeparam name="T">The type of enum to parse</typeparam>
        /// <param name="message">The string enum</param>
        /// <returns>The enum</returns>
        public static T ParseEnum<T>(this string message)
        {
            if (!typeof(T).IsEnum)
                return default(T);
            return (T)message.ChangeType(typeof(T));
        }

        /// <summary>
        /// Nullable safe method of converting object between types.
        /// </summary>
        /// <param name="value">The object to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>The converted object</returns>
        public static object ChangeType(this object value, System.Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            if (t.IsEnum)
            {
                try
                {
                    return Enum.ToObject(t, Convert.ChangeType(value, typeof(int)));
                }
                catch
                {
                    return Enum.Parse(t, (string)Convert.ChangeType(value, typeof(string)), true);
                }
            }
            return Convert.ChangeType(value, t);
        }

        /// <summary>
        /// Trys to parse a string as an enum either from the name or integer value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static bool TryParseEnum<T>(this string message, out T res)
        {
            var type = typeof(T);
            var t = Nullable.GetUnderlyingType(type) ?? type;
            if (!t.IsEnum)
            {
                res = default(T);
                return false;
            }
            try
            {
                res = (T)Enum.ToObject(t, Convert.ChangeType(message, typeof(int)));
            }
            catch
            {
                try
                {
                    res = (T)Enum.Parse(t, message.Replace(" ", ""), true);
                }
                catch
                {
                    res = default(T);
                    return false;
                }
            }

            return true;
        }

        private static ConsoleColor ColorFromCarat(char item)
        {
            switch (item)
            {
                case 'b': return ConsoleColor.Blue;
                case 'c': return ConsoleColor.Cyan;
                case 'g': return ConsoleColor.Gray;
                case 'e': return ConsoleColor.Green;
                case 'm': return ConsoleColor.Magenta;
                case 'r': return ConsoleColor.Red;
                case 'w': return ConsoleColor.White;
                case 'y': return ConsoleColor.Yellow;
                default: return ConsoleColor.Black;
            }
        }

        private static bool Working = false;

        /// <summary>
        /// Coloured console writer takes letters after ^ and converts into different colors.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="endline"></param>
        public static void ColouredConsole(this string data, bool endline = true)
        {
            if (Working)
            {
                System.Threading.Thread.Sleep(1);
                ColouredConsole(data, endline);
                return;
            }

            Working = true;

            var start = Console.ForegroundColor;
            var parts = (data.StartsWith("^") ? data : "w" + data).Split('^');

            foreach(var part in parts)
            {
                if (part.Length <= 0)
                    continue;
                var c = part[0];
                var color = ColorFromCarat(c);

                var sub = part.Substring(1);
                Console.ForegroundColor = color;
                Console.Write(sub);
            }
            if (endline)
                Console.WriteLine();

            Console.ForegroundColor = start;
            Working = false;
        }
    }
}
