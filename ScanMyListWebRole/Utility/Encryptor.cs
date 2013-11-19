namespace SynchWebRole.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Security.Cryptography;
    using System.Text;

    public class Encryptor
    {
        public static string Generate512Hash(string password)
        {
            SHA512 alg = SHA512.Create();

            return Encoding.ASCII.GetString(alg.ComputeHash(GenerateBytes(password)));
        }
        public static string GenerateSimpleHash(string password)
        {
            MD5 alg = MD5.Create();
            string hash = "";
            byte[] hashes = alg.ComputeHash(GenerateBytes(password));
            foreach (byte b in hashes)
                hash += b.ToString("X");

            return hash;
            
        }

        private static byte[] GenerateBytes(string key)
        {
            UTF8Encoding encoding = new UTF8Encoding();

            return encoding.GetBytes(key);
        }
    }
}