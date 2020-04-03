using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SpiritTime.Shared.Helper
{
    public class Crypt
    {
        public static string Encrypt(string password)
        {
            var provider = MD5.Create();
            string salt = "S0m3R@nd0mSalt";
            byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(salt + password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
