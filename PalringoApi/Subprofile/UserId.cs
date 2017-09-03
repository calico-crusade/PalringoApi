using Newtonsoft.Json;
using PalringoApi.Subprofile;
using PalringoApi.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;

namespace PalringoApi
{
    /// <summary>
    /// User Id mask to add helper classes
    /// </summary>
    [JsonConverter(typeof(MaskedIntConverter))]
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public struct UserId : IConvertible
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        private static Dictionary<UserId, Bitmap> _cachedAvatars = new Dictionary<UserId, Bitmap>();

        private int _value { get; set; }

        /// <summary>
        /// Gets a user profile from the bot
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public User GetUser(PalBot bot)
        {
            return bot.Information.UserCache.ContainsKey(this) ? bot.Information.UserCache[this] : null;
        }

        /// <summary>
        /// Gets an avatar from the bot
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Bitmap GetAvatar(int size)
        {
            try
            {
                if (_cachedAvatars.ContainsKey(_value) && _cachedAvatars[_value].Width >= size)
                    return new Bitmap(_cachedAvatars[_value], size, size);

                var data = new WebClient().DownloadData($"http://palringo.com/showavatar.php?id={_value}&size={size}");
                var avi = ImageService.BitmapFromBytes(data);
                if (_cachedAvatars.ContainsKey(_value))
                    _cachedAvatars[_value] = avi;
                else
                    _cachedAvatars.Add(_value, avi);
                return new Bitmap(avi, size, size);
            }
            catch
            {
                return new Bitmap(size, size);
            }
        }

        /// <summary>
        /// Trys to parse a string to a userid
        /// </summary>
        /// <param name="idin"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool TryParse(string idin, out UserId id)
        {
            int oid;
            bool isParsed = int.TryParse(idin, out oid);

            id = oid;
            return isParsed; 
        }

        /// <summary>
        /// trys to parse a string to a userid and get profile
        /// </summary>
        /// <param name="idin"></param>
        /// <param name="bot"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool TryParse(string idin, PalBot bot, out UserId id)
        {
            int oid;
            bool isParsed = int.TryParse(idin, out oid);
            if (isParsed)
            {
                id = oid;
                return true;
            }

            var usrs = bot.Information.UserCache.Where(t => t.Value.Nickname.ToLower().Trim() == idin.ToLower().Trim()).ToArray();
            if (usrs.Length > 0)
            {
                id = usrs.Last().Key;
                return true;
            }

            id = -1;
            return false;
        }

        /// <summary>
        /// Converts UserId to an int
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator int(UserId id)
        {
            return id._value;
        }

        /// <summary>
        /// Converts int into a UserId
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator UserId(int id)
        {
            return new UserId { _value = id };
        }

        /// <summary>
        /// Converts User id to a string
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator string(UserId id)
        {
            return ((int)id).ToString();
        }

        /// <summary>
        /// converts string to a User id
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator UserId(string id)
        {
            return int.Parse(id);
        }

        /// <summary>
        /// evaluates two User ids
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(UserId i1, UserId i2)
        {
            return i1._value == i2._value;
        }

        /// <summary>
        /// evaluates two User ids to not match
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(UserId i1, UserId i2)
        {
            return i1._value != i2._value;
        }

        /// <summary>
        /// evalutes a User id and an int
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(UserId i1, int i2)
        {
            return i1._value == i2;
        }

        /// <summary>
        /// evalutes a User id and an int to not match
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(UserId i1, int i2)
        {
            return i1._value != i2;
        }

        /// <summary>
        /// evaluates a Userid and a string
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(UserId i1, string i2)
        {
            return i1 == i2;
        }

        /// <summary>
        /// Evaluates a Userid and a string to not match
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(UserId i1, string i2)
        {
            return i1 != i2;
        }


        /// <summary>
        /// Evaluates an int and a Userid
        /// </summary>
        /// <param name="i2"></param>
        /// <param name="i1"></param>
        /// <returns></returns>
        public static bool operator ==(int i2, UserId i1)
        {
            return i1._value == i2;
        }

        /// <summary>
        /// Evaluates an int and a Userid not to match
        /// </summary>
        /// <param name="i2"></param>
        /// <param name="i1"></param>
        /// <returns></returns>
        public static bool operator !=(int i2, UserId i1)
        {
            return i1._value != i2;
        }

        /// <summary>
        /// Evaluates a string and a User id
        /// </summary>
        /// <param name="i2"></param>
        /// <param name="i1"></param>
        /// <returns></returns>
        public static bool operator ==(string i2, UserId i1)
        {
            return i1 == i2;
        }

        /// <summary>
        /// Evaluates a string and a User id to not match
        /// </summary>
        /// <param name="i2"></param>
        /// <param name="i1"></param>
        /// <returns></returns>
        public static bool operator !=(string i2, UserId i1)
        {
            return i1 != i2;
        }

        /// <summary>
        /// overrides the .Equals function
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Overrides GetHashCode()
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Overrides tostring
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Int32;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToInt32(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public string ToString(IFormatProvider provider)
        {
            return _value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(_value, typeof(UserId), provider);
        }
    }
}
