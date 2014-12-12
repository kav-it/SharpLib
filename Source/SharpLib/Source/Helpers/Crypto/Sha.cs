using System;
using System.Security.Cryptography;

namespace SharpLib
{
    /// <summary>
    /// Хэширование SHA-1/256/384/512
    /// </summary>
    public class Sha
    {
        #region Перечисления

        public enum Algorithm
        {
            SHA1,

            SHA256,

            SHA384,

            SHA512
        };

        #endregion

        #region Методы

        /// <summary>
        /// Расчет хэша
        /// </summary>
        public static string Hash(string text, Algorithm algorithm = Algorithm.SHA1)
        {
            // Определение необходимого алгоритма хэширования
            HashAlgorithm hasher;
            switch (algorithm)
            {
                case Algorithm.SHA1:
                    hasher = new SHA1Managed();
                    break;
                case Algorithm.SHA256:
                    hasher = new SHA256Managed();
                    break;
                case Algorithm.SHA384:
                    hasher = new SHA384Managed();
                    break;
                default:
                    hasher = new SHA512Managed();
                    break;
            }

            byte[] bufIn = text.ToBufferEx();
            byte[] bufOut = hasher.ComputeHash(bufIn);

            string hash = bufOut.ToAsciiEx(string.Empty).ToLower();

            return hash;
        }

        #endregion
    }
}