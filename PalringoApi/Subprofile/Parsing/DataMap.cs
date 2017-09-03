using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PalringoApi.Subprofile.Parsing
{
    /// <summary>
    /// Used for translating bytebuffers into deserializable data
    /// Not going to comment anything I don't use.
    /// Note: Wripped from Java Client.
    /// To be honest, didn't comment any of it. 
    /// If you need to use, contact me (alec)
    /// </summary>
    [Serializable]
    public class DataMap : IEnumerable
    {
        private readonly Dictionary<string, byte[]> hiddenData;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataMap()
        {
            hiddenData = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public DataMap(string data)
            : this(Static.PalringoEncoding.GetBytes(data)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataMap_0"></param>
        public DataMap(DataMap dataMap_0)
        {
            hiddenData = new Dictionary<string, byte[]>(dataMap_0.hiddenData);
        }

        /// <summary>
        /// Get DataMap from byte array
        /// </summary>
        /// <param name="fromData"></param>
        public DataMap(byte[] fromData)
        {
            hiddenData = new Dictionary<string, byte[]>();
            Deserialize(fromData, 0, fromData.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        public uint CacheSize
        {
            get { return 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return hiddenData.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, byte[]> Data
        {
            get { return hiddenData; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> StrData => Data.ToDictionary(t => t.Key, t => Static.PalringoEncoding.GetString(t.Value));
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return hiddenData.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CalculateSize()
        {
            int length = 0;
            foreach (KeyValuePair<string, byte[]> dictionary0 in hiddenData)
            {
                int length1 = dictionary0.Value.Length;
                do
                {
                    int num = (length1 > 65535 ? 65535 : length1);
                    int num1 = num;
                    length = length + dictionary0.Key.Length;
                    length = length + 3;
                    length = length + num1;
                    length1 = length1 - num1;
                } while (length1 > 0);
            }
            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            hiddenData.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public bool ContainsKey(string string_0)
        {
            return hiddenData.ContainsKey(string_0.ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void Deserialize(byte[] data, int start, int length)
        {
            hiddenData.Clear();
            var byteBuffer = new ByteBuffer(data);
            int num = start;
            string str = "";
            int num1 = start + length;
            while (num < num1)
            {
                if (data[num] == 0)
                {
                    break;
                }
                string lower = byteBuffer.GetString(num).ToLower();
                num = num + lower.Length + 1;
                if (num >= num1)
                {
                    throw new AttributeErrorException();
                }
                if (num + 2 > num1)
                {
                    throw new Exception1();
                }
                else
                {
                    int num2 = num;
                    num = num2 + 1;
                    int num3 = data[num2] << 8;
                    int num4 = num;
                    num = num4 + 1;
                    num3 = num3 + data[num4];
                    if (num + num3 > num1)
                    {
                        throw new Exception2();
                    }
                    else
                    {
                        //	if (str.Length <= 0 || str.CompareTo(lower) != 0)
                        if (str.Length <= 0 || string.CompareOrdinal(str, lower) != 0)
                        {
                            var numArray = new byte[num3];
                            Array.Copy(byteBuffer.Buffer, num, numArray, 0, num3);
                            hiddenData[lower] = numArray;
                        }
                        else
                        {
                            byte[] item = hiddenData[lower];
                            var numArray1 = new byte[item.Length + num3];
                            item.CopyTo(numArray1, 0);
                            Array.Copy(byteBuffer.Buffer, num, numArray1, item.Length, num3);
                            hiddenData[lower] = numArray1;
                        }
                        str = lower;
                        num = num + num3;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetNestedAttrib(string path)
        {
            return GetNestedAttrib(path, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public string GetNestedAttrib(string path, string string_0)
        {
            if (string_0 != null)
            {
                path = string.Concat(path, ".", string_0);
            }
            var chrArray = new[] { '.' };
            string[] strArrays = path.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
            DataMap valueMap = this;
            int num = 0;
            while (num < strArrays.Length)
            {
                if (num != strArrays.Length - 1)
                {
                    valueMap = valueMap.GetValueMap(strArrays[num]);
                    if (valueMap == null)
                    {
                        return null;
                    }
                    num++;
                }
                else
                {
                    return valueMap.GetValue(strArrays[num]);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public string GetValue(string string_0)
        {
            string_0 = string_0.ToLower();
            if (!hiddenData.ContainsKey(string_0))
            {
                return string.Empty;
            }
            byte[] item = hiddenData[string_0];
            return Static.PalringoEncoding.GetString(item, 0, item.Length).GetDisplayString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public float GetValueFloat(string string_0)
        {
            string_0 = string_0.ToLower();
            if (hiddenData.ContainsKey(string_0))
            {
                float single;
                byte[] item = hiddenData[string_0];
                string str = Static.PalringoEncoding.GetString(item, 0, item.Length);
                if (float.TryParse(str, out single))
                {
                    return single;
                }
            }
            return 0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public int GetValueInt(string string_0)
        {
            string_0 = string_0.ToLower();
            if (hiddenData.ContainsKey(string_0))
            {
                int num;
                byte[] item = hiddenData[string_0];
                string str = Static.PalringoEncoding.GetString(item, 0, item.Length);
                if (int.TryParse(str, out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public long GetValueInt64(string string_0)
        {
            string_0 = string_0.ToLower();
            if (hiddenData.ContainsKey(string_0))
            {
                long num;
                byte[] item = hiddenData[string_0];
                string str = Static.PalringoEncoding.GetString(item, 0, item.Length);
                if (long.TryParse(str, out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public DataMap GetValueMap(string string_0)
        {
            try
            {
                string_0 = string_0.ToLower();
                if (!hiddenData.ContainsKey(string_0))
                {
                    return null;
                }
                var dataMaps = new DataMap();
                dataMaps.Deserialize(hiddenData[string_0], 0, hiddenData[string_0].Length);
                return dataMaps;
            }
            catch { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public DataMap GetValueMapAll(string str)
        {
            var items = hiddenData.Where(t => t.Key.StartsWith(str) || t.Key.EndsWith(str));
            foreach (var item in items)
            {
                var map = new DataMap();
                map.Deserialize(item.Value, 0, item.Value.Length);
                return map;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public DataMap GetEndValueMap(string str)
        {
            foreach (var item in hiddenData)
            {
                if (item.Key.EndsWith(str))
                {
                    var end = new DataMap();
                    end.Deserialize(item.Value, 0, item.Value.Length);
                    return end;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public byte[] GetValueRaw(string string_0)
        {
            string_0 = string_0.ToLower();
            if (!hiddenData.ContainsKey(string_0))
            {
                return null;
            }
            return hiddenData[string_0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public uint GetValueUInt(string string_0)
        {
            string_0 = string_0.ToLower();
            if (hiddenData.ContainsKey(string_0))
            {
                uint num;
                byte[] item = hiddenData[string_0];
                string str = Static.PalringoEncoding.GetString(item, 0, item.Length);
                if (uint.TryParse(str, out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public uint GetValueUInt32(string string_0)
        {
            string_0 = string_0.ToLower();
            if (hiddenData.ContainsKey(string_0))
            {
                uint num;
                byte[] item = hiddenData[string_0];
                string str = Static.PalringoEncoding.GetString(item, 0, item.Length);
                if (uint.TryParse(str, out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <returns></returns>
        public ulong GetValueUInt64(string string_0)
        {
            string_0 = string_0.ToLower();
            if (hiddenData.ContainsKey(string_0))
            {
                ulong num;
                byte[] item = hiddenData[string_0];
                string str = Static.PalringoEncoding.GetString(item, 0, item.Length);
                if (ulong.TryParse(str, out num))
                {
                    return num;
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        public void RemoveEntry(string string_0)
        {
            string_0 = string_0.ToLower();
            if (hiddenData.ContainsKey(string_0))
            {
                hiddenData.Remove(string_0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            int num1 = CalculateSize();
            var numArray = new byte[num1];
            int length = 0;
            foreach (KeyValuePair<string, byte[]> dictionary0 in hiddenData)
            {
                int num2 = 0;
                byte[] value = dictionary0.Value;
                do
                {
                    int length1 = dictionary0.Value.Length - num2;
                    int num = (length1 > 65535 ? 65535 : length1);
                    int num3 = num;
                    byte[] bytes = Static.PalringoEncoding.GetBytes(dictionary0.Key);
                    bytes.CopyTo(numArray, length);
                    length = length + bytes.Length;
                    int num4 = length;
                    length = num4 + 1;
                    numArray[num4] = 0;
                    int num5 = length;
                    length = num5 + 1;
                    numArray[num5] = (byte)(num3 >> 8);
                    int num6 = length;
                    length = num6 + 1;
                    numArray[num6] = (byte)(num3 & 255);
                    Array.Copy(value, num2, numArray, length, num3);
                    length = length + num3;
                    num2 = num2 + num3;
                } while (num2 < dictionary0.Value.Length);
            }
            return numArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void SetNestedAttribute(string path, byte[] data)
        {
            int num = path.LastIndexOf(".", StringComparison.Ordinal);
            string str = path.Substring(num + 1);
            string str1 = path.Substring(0, num);
            SetNestedAttribute(str1, str, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void SetNestedAttribute(string path, string data)
        {
            int num = path.LastIndexOf(".", StringComparison.Ordinal);
            string str = path.Substring(num + 1);
            string str1 = path.Substring(0, num);
            SetNestedAttribute(str1, str, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="string_0"></param>
        /// <param name="string_1"></param>
        public void SetNestedAttribute(string path, string string_0, string string_1)
        {
            SetNestedAttribute(path, string_0, Static.PalringoEncoding.GetBytes(string_1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="string_0"></param>
        /// <param name="byte_0"></param>
        public void SetNestedAttribute(string path, string string_0, byte[] byte_0)
        {
            var chrArray = new[] { '.' };
            string[] strArrays = path.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
            var keyValuePairs = new Stack<KeyValuePair<string, DataMap>>(strArrays.Length);
            DataMap valueMap = this;
            foreach (string t in strArrays)
            {
                valueMap = valueMap.GetValueMap(t) ?? new DataMap();
                keyValuePairs.Push(new KeyValuePair<string, DataMap>(t, valueMap));
            }
            KeyValuePair<string, DataMap> keyValuePair = keyValuePairs.Peek();
            keyValuePair.Value.SetValueRaw(string_0, byte_0);
            while (keyValuePairs.Count > 1)
            {
                KeyValuePair<string, DataMap> keyValuePair1 = keyValuePairs.Pop();
                KeyValuePair<string, DataMap> keyValuePair2 = keyValuePairs.Peek();
                keyValuePair2.Value.SetValueRaw(keyValuePair1.Key, keyValuePair1.Value.Serialize());
            }
            KeyValuePair<string, DataMap> keyValuePair3 = keyValuePairs.Peek();
            KeyValuePair<string, DataMap> keyValuePair4 = keyValuePairs.Peek();
            SetValueRaw(keyValuePair3.Key, keyValuePair4.Value.Serialize());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <param name="string_1"></param>
        public void SetValue(string string_0, string string_1)
        {
            string_0 = string_0.ToLower();
            if (string_1 != null)
            {
                hiddenData[string_0] = Static.PalringoEncoding.GetBytes(string_1);
            }
            else
            {
                hiddenData[string_0] = Static.PalringoEncoding.GetBytes(string.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <param name="int_0"></param>
        public void SetValue(string string_0, int int_0)
        {
            string_0 = string_0.ToLower();
            hiddenData[string_0] = Static.PalringoEncoding.GetBytes(int_0.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <param name="uint_0"></param>
        public void SetValue(string string_0, uint uint_0)
        {
            string_0 = string_0.ToLower();
            hiddenData[string_0] = Static.PalringoEncoding.GetBytes(uint_0.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <param name="long_0"></param>
        public void SetValue(string string_0, long long_0)
        {
            string_0 = string_0.ToLower();
            hiddenData[string_0] = Static.PalringoEncoding.GetBytes(long_0.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <param name="ulong_1"></param>
        public void SetValue(string string_0, ulong ulong_1)
        {
            string_0 = string_0.ToLower();
            hiddenData[string_0] = Static.PalringoEncoding.GetBytes(ulong_1.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        /// <param name="byte_0"></param>
        public void SetValueRaw(string string_0, byte[] byte_0)
        {
            string_0.ToLower();
            hiddenData[string_0] = byte_0;
        }

        /// <summary>
        /// 
        /// </summary>
        public class AttributeErrorException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            public AttributeErrorException()
                : base("Attribute name is incomplete")
            {
            }
        }

        private class Exception1 : Exception
        {
            public Exception1()
                : base("Length of attribute value is incomplete")
            {
            }
        }

        private class Exception2 : Exception
        {
            public Exception2()
                : base("Invalid attribute value")
            {
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Payload"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Deserialize(string Payload)
        {
            Dictionary<string, string> Attrs = new Dictionary<string, string>();
            string SubPayload = Payload;
            while (true)
            {
                try
                {
                    int temp = SubPayload.IndexOf('\0'); if (temp < 0) break;
                    string attribute = SubPayload.Substring(0, temp);

                    SubPayload = SubPayload.Substring(temp + 1);

                    string len = SubPayload.Substring(0, 2);
                    string hex = "";
                    foreach (char c in len)
                    {
                        int tmp = c;
                        hex += String.Format("{0:x2}", Convert.ToUInt32(tmp.ToString()));
                    }
                    int length = int.Parse(hex, NumberStyles.AllowHexSpecifier);

                    SubPayload = SubPayload.Substring(2);

                    string inner = SubPayload.Substring(0, length);
                    SubPayload = SubPayload.Substring(length);

                    Attrs.Add(attribute.ToLower(), inner);
                }
                catch (Exception)
                {

                }
            }
            return Attrs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataMap> EnumerateMaps()
        {
            foreach (var item in Data)
            {
                var m = GetValueMap(item.Key);
                if (m != null)
                {
                    foreach (var t in m.EnumerateMaps())
                        yield return t;
                }
            }
            yield return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public string ToString(string begin)
        {
            string o = "";

            foreach(var value in StrData)
            {
                var m = GetValueMap(value.Key);
                if (m != null)
                    o += begin + value.Key + ":\r\n" + m.ToString(begin + "\t") + "\r\n";
                else
                    o += begin + value.Key + ": " + value.Value + "\r\n";
            }

            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString("");
        }
    }
}
