using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharpLib
{
    public class Aes
    {
        private const string SALT_DEFAULT = @"codeofrussia.ru";

        #region Методы

        /// <summary>
        /// Кодирование данных
        /// </summary>
        public static string Encrypt(string plainText, string password)
        {
            return Encrypt(plainText, password, SALT_DEFAULT, "SHA1", 2, "Oas5!73m*a_G0dxi", 256);
        }

        /// <summary>
        /// Кодирование данных
        /// </summary>
        public static string Encrypt(string plainText, string password, string salt, string hashAlgorithm, int passwordIterations, string initialVector, int keySize)
        {
            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);

            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes);
            var memStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            var cipherTextBytes = memStream.ToArray();
            memStream.Close();
            cryptoStream.Close();

            string text = Convert.ToBase64String(cipherTextBytes);

            return text;
        }

        /// <summary>
        /// Декодирование данных
        /// </summary>
        public static string Decrypt(string cipherText, string password, string salt, string hashAlgorithm, int passwordIterations, string initialVector, int keySize)
        {
            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            var derivedPassword = new Rfc2898DeriveBytes(password, saltValueBytes, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            var decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes);
            var memStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new Byte[cipherTextBytes.Length];
            int byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memStream.Close();
            cryptoStream.Close();

            string text = Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);

            return text;
        }

        /// <summary>
        /// Декодирование данных
        /// </summary>
        public static string Decrypt(string cipherText, string password)
        {
            return Decrypt(cipherText, password, SALT_DEFAULT, "SHA1", 2, "Oas5!73m*a_G0dxi", 256);
        }

        #endregion
    }
}