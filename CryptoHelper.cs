using System;
using System.IO;
using System.Security.Cryptography;

namespace FoxNote
{
    public static class CryptoHelper
    {
        public static byte[] Encrypt(string plainText, string password)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] salt = GenerateSalt();
                var key = new Rfc2898DeriveBytes(password, salt, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                using (var ms = new MemoryStream())
                {
                    ms.Write(salt, 0, salt.Length);
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return ms.ToArray();
                }
            }
        }

        public static string Decrypt(byte[] cipherData, string password)
        {
            byte[] salt = new byte[16];
            Array.Copy(cipherData, 0, salt, 0, salt.Length);

            using (Aes aes = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(password, salt, 10000);
                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                using (var ms = new MemoryStream(cipherData, 16, cipherData.Length - 16))
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            return salt;
        }
    }
}
