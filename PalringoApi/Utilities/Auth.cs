using System;
using System.Security.Cryptography;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// This generates passwords the way pal likes
    /// </summary>
    public class Auth
    {
        /// <summary>
        /// Generates the passwords based off of the salt from pal
        /// </summary>
        /// <param name="keyAndIV"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] generateAuth(byte[] keyAndIV, byte[] password)
        {
            byte[] IV = new byte[8];
            int i = 0;

            for (i = 16; i < 24; i++)
            {
                IV[i - 16] = keyAndIV[i];
            }

            byte[] authKey = doubleMd5(password, IV);
            byte[] rnd = generateRandomBytes(16);
            byte[] sessionKey = doubleMd5(password, rnd);

            byte[] dataToEncrypt = new byte[32];

            for (i = 0; i < 16; i++)
            {
                dataToEncrypt[i] = keyAndIV[i];
                dataToEncrypt[i + 16] = rnd[i];
            }


            byte[] result = salsa20(IV, authKey, dataToEncrypt);
            return result;
        }

        private static byte[] md5(byte[] input)
        {
            MD5 m = MD5.Create();
            return m.ComputeHash(input);
        }

        private static byte[] doubleMd5(byte[] abyte0, byte[] abyte1)
        {
            byte[] abyte2 = md5(abyte0);
            byte[] abyte3 = new byte[abyte2.Length + abyte1.Length];
            Array.Copy(abyte2, 0, abyte3, 0, abyte2.Length);
            Array.Copy(abyte1, 0, abyte3, abyte2.Length, abyte1.Length);
            return md5(abyte3);
        }

        private static byte[] salsa20(byte[] iv, byte[] key, byte[] data)
        {
            Salsa20 s = new Salsa20();

            ICryptoTransform enc = s.CreateEncryptor(key, iv);

            byte[] result = new byte[32];

            enc.TransformBlock(data, 0, data.Length, result, 0);

            return result;

        }

        private static byte[] generateRandomBytes(int i)
        {
            byte[] result = new byte[i];

            RandomNumberGenerator.Create().GetBytes(result);

            return result;
        }
    }
}
