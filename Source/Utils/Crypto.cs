//*****************************************************************************
//
// Имя файла    : 'Crypto.cs'
// Заголовок    : Модуль криптографии
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 05/06/2012
//
//*****************************************************************************

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharpLib
{

    #region Класс MD5

    /// <summary>
    /// Хэширование MD5
    /// </summary>
    public class Md5
    {
        #region Методы

        public static String Hash(String text)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            Byte[] bufIn = text.ToBufferEx();
            Byte[] bufOut = md5Hasher.ComputeHash(bufIn);

            String hash = Conv.BufferToString(bufOut);

            return hash;
        }

        #endregion
    }

    #endregion Класс MD5

    #region Класс Aes

    public class Aes
    {
        #region Методы

        /// <summary>
        /// Кодирование данных
        /// </summary>
        public static string Encrypt(string plainText, string password)
        {
            return Encrypt(plainText, password, "_Kavit_", "SHA1", 2, "Oas5!73m*a_G0dxi", 256);
        }

        /// <summary>
        /// Кодирование данных
        /// </summary>
        public static String Encrypt(String plainText, String password, String salt, String hashAlgorithm, int passwordIterations, String initialVector, int keySize)
        {
            Byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            Byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            Byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            Byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes);
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            Byte[] cipherTextBytes = memStream.ToArray();
            memStream.Close();
            cryptoStream.Close();

            String text = Convert.ToBase64String(cipherTextBytes);

            return text;
        }

        /// <summary>
        /// Декодирование данных
        /// </summary>
        public static String Decrypt(String cipherText, String password, String salt, String hashAlgorithm, int passwordIterations, String initialVector, int keySize)
        {
            Byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            Byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            Byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            Byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes);
            MemoryStream memStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
            Byte[] plainTextBytes = new Byte[cipherTextBytes.Length];
            int byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memStream.Close();
            cryptoStream.Close();

            String text = Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);

            return text;
        }

        /// <summary>
        /// Декодирование данных
        /// </summary>
        public static String Decrypt(String cipherText, String password)
        {
            return Decrypt(cipherText, password, "_Kavit_", "SHA1", 2, "Oas5!73m*a_G0dxi", 256);
        }

        #endregion
    }

    #endregion Класс Aes
}