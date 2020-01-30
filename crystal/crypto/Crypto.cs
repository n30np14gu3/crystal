using System;
using System.Security.Cryptography;

namespace crystal.crypto
{
    /// <summary>
    /// Contains cryptography methods
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// Computing SHA256 Controll sum
        /// </summary>
        /// <param name="data">raw data to compute</param>
        /// <returns>string hash</returns>
        public static string Sha256(byte[] data) => BitConverter.ToString(new SHA256CryptoServiceProvider().ComputeHash(data)).Replace("-", "");
    }
}
