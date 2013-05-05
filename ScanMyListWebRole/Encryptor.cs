namespace ScanMyListWebRole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Security.Cryptography;
    using System.Text;

    public class Encryptor
    {
        public static string GenerateHash(string password)
        {
            SHA512 alg = SHA512.Create();

            return Encoding.ASCII.GetString(alg.ComputeHash(GenerateBytes(password)));
        }

        private static byte[] GenerateBytes(string key)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            return encoding.GetBytes(key);
        }
    }
}