using System;
using System.Text;

namespace PalringoApi.Subprofile.Parsing
{
    /// <summary>
    /// Addes the ability to de-cluge palringo sub profile info
    /// Not going to comment, you should never need to use this directly
    /// <see cref="DataMap"/>
    /// Note: this was ripped from the java client. so it is pals decompiled code.
    /// </summary>
    public class ByteBuffer
    {
        /// <summary>
        /// 
        /// </summary>
        public static Encoding encoding = Static.PalringoEncoding;
        
        private const int AllocBlockSize = 512;

        
        private static int ByteBufferAllocs;

        private int int_0;

        private int int_1;

        /// <summary>
        /// 
        /// </summary>
        public ByteBuffer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte_1"></param>
        public ByteBuffer(byte[] byte_1)
        {
            Buffer = byte_1;
            int_0 = byte_1.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public ByteBuffer(uint length)
        {
            Buffer = new byte[length];
            int_0 = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get { return int_0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ReadIndex
        {
            get { return int_1; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte_1"></param>
        public void Append(byte[] byte_1)
        {
            method_0(byte_1.Length);
            byte_1.CopyTo(Buffer, int_0);
            ByteBuffer int0 = this;
            int0.int_0 = int0.int_0 + byte_1.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte_1"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void Append(byte[] byte_1, int start, int length)
        {
            method_0(length);
            Array.Copy(byte_1, start, Buffer, int_0, length);
            ByteBuffer int0 = this;
            int0.int_0 = int0.int_0 + length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_0"></param>
        public void AppendString(string string_0)
        {
            if (string_0 == null)
            {
                Append(BitConverter.GetBytes(0));
                return;
            }
            else
            {
                byte[] bytes = encoding.GetBytes(string_0);
                Append(BitConverter.GetBytes(bytes.Length));
                Append(bytes);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            Buffer = new byte[0];
            int_0 = 0;
            int_1 = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numBytes"></param>
        public void CullFront(int numBytes)
        {
            if (numBytes < int_0)
            {
                Array.Copy(Buffer, numBytes, Buffer, 0, int_0 - numBytes);
                ByteBuffer int0 = this;
                int0.int_0 = int0.int_0 - numBytes;
            }
            else
            {
                int_0 = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="additionalSize"></param>
        public void FitData(int additionalSize)
        {
            if (Buffer.Length != int_0 + additionalSize)
            {
                int int0 = int_0 + additionalSize;
                var numArray = new byte[int0];
                Array.Copy(Buffer, numArray, int_0);
                Buffer = numArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="char_0"></param>
        /// <returns></returns>
        public int GetIndexOfChar(char char_0)
        {
            return GetIndexOfChar(char_0, 0, int_0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="char_0"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int GetIndexOfChar(char char_0, int start)
        {
            return GetIndexOfChar(char_0, start, int_0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="char_0"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int GetIndexOfChar(char char_0, int start, int length)
        {
            int int0 = start + length;
            if (int0 > int_0)
            {
                int0 = int_0;
            }
            int num = start;
            while (num < int0)
            {
                if (Buffer[num] != char_0)
                {
                    num++;
                }
                else
                {
                    return num;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GetString(int start, int length)
        {
            if (Buffer == null || Buffer.Length <= 0)
            {
                return string.Empty;
            }
            return encoding.GetString(Buffer, start, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public string GetString(int start)
        {
            int num = start;
            while (num < int_0)
            {
                if (Buffer[num] == 0)
                {
                    return encoding.GetString(Buffer, start, num - start);
                }
                num++;
            }
            return encoding.GetString(Buffer, start, int_0 - start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="int_2"></param>
        private void method_0(int int_2)
        {
            int num = (Buffer != null ? Buffer.Length : 0);
            int num1 = num;
            if (num1 - int_0 < int_2)
            {
                ByteBufferAllocs = ByteBufferAllocs + 1;
                int num2 = Math.Max(int_2 - (num1 - int_0), 512);
                var numArray = new byte[num1 + num2];
                if (num1 > 0)
                {
                    Array.Copy(Buffer, numArray, int_0);
                }
                Buffer = numArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            bool flag = BitConverter.ToBoolean(Buffer, int_1);
            ByteBuffer int1 = this;
            int1.int_1 = int1.int_1 + 1;
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            float single = BitConverter.ToSingle(Buffer, int_1);
            ByteBuffer int1 = this;
            int1.int_1 = int1.int_1 + 4;
            return single;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            int num = BitConverter.ToInt32(Buffer, int_1);
            ByteBuffer int1 = this;
            int1.int_1 = int1.int_1 + 4;
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            long num = BitConverter.ToInt64(Buffer, int_1);
            ByteBuffer int1 = this;
            int1.int_1 = int1.int_1 + 8;
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            int num = BitConverter.ToInt32(Buffer, int_1);
            ByteBuffer int1 = this;
            int1.int_1 = int1.int_1 + 4;
            string str = encoding.GetString(Buffer, int_1, num);
            ByteBuffer byteBuffer = this;
            byteBuffer.int_1 = byteBuffer.int_1 + num;
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            uint num = BitConverter.ToUInt32(Buffer, int_1);
            ByteBuffer int1 = this;
            int1.int_1 = int1.int_1 + 4;
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            ulong num = BitConverter.ToUInt64(Buffer, int_1);
            ByteBuffer int1 = this;
            int1.int_1 = int1.int_1 + 8;
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public void Reserve(uint size)
        {
            if (Buffer == null || Buffer.Length < (decimal)size)
            {
                var numArray = new byte[size];
                if (Buffer == null)
                {
                    int_0 = 0;
                }
                else
                {
                    Array.Copy(Buffer, numArray, Buffer.Length);
                    int_0 = Buffer.Length;
                }
                Buffer = numArray;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numBytes"></param>
        /// <param name="readPtr"></param>
        public void Skip(int numBytes, bool readPtr)
        {
            if (!readPtr)
            {
                ByteBuffer int0 = this;
                int0.int_0 = int0.int_0 + numBytes;
            }
            else
            {
                ByteBuffer int1 = this;
                int1.int_1 = int1.int_1 + numBytes;
            }
        }
    }
}
