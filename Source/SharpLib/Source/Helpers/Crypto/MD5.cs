using System.Security.Cryptography;

namespace SharpLib
{
    /// <summary>
    /// Хэширование MD5
    /// </summary>
    public class Md5
    {
        #region Методы

        public static string Hash(string text)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] bufIn = text.ToBufferEx();
            byte[] bufOut = md5Hasher.ComputeHash(bufIn);

            string hash = bufOut.ToAsciiEx(string.Empty).ToLower();

            return hash;
        }

        #endregion
    }
}