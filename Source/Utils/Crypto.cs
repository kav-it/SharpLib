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
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SharpLib
{
    #region Класс MD5
    /// <summary>
    /// Хэширование MD5
    /// </summary>
    public class Md5
    {
        public static String Hash (String text)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            Byte[] bufIn = text.ToBufferEx();
            Byte[] bufOut = md5Hasher.ComputeHash(bufIn);

            String hash = Conv.BufferToString(bufOut);

            return hash;
        }
    }
    #endregion Класс MD5

    #region Класс Aes
    public class Aes
    {
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
        public static String Encrypt(String PlainText, String Password, String Salt, String HashAlgorithm, int PasswordIterations, String InitialVector, int keySize)
        {
            Byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            Byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            Byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
           
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            Byte[] KeyBytes = DerivedPassword.GetBytes(keySize / 8);
            RijndaelManaged SymmetricKey = new RijndaelManaged();
            SymmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes);
            MemoryStream MemStream = new MemoryStream();
            CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);
            CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
            CryptoStream.FlushFinalBlock();
            Byte[] CipherTextBytes = MemStream.ToArray();
            MemStream.Close();
            CryptoStream.Close();

            String text = Convert.ToBase64String(CipherTextBytes);

            return text;
        }
        /// <summary>
        /// Декодирование данных
        /// </summary>
        public static String Decrypt(String CipherText, String Password, String Salt, String HashAlgorithm, int PasswordIterations, String InitialVector, int keySize)
        {
            Byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            Byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            Byte[] CipherTextBytes = Convert.FromBase64String(CipherText);

            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            Byte[] KeyBytes = DerivedPassword.GetBytes(keySize / 8);
            RijndaelManaged SymmetricKey = new RijndaelManaged();
            SymmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes);
            MemoryStream MemStream = new MemoryStream(CipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);
            Byte[] PlainTextBytes = new Byte[CipherTextBytes.Length];
            int ByteCount = cryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
            MemStream.Close();
            cryptoStream.Close();

            String text = Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);

            return text;
        }
        /// <summary>
        /// Декодирование данных
        /// </summary>
        public static String Decrypt(String cipherText, String password)
        {
            return Decrypt(cipherText, password, "_Kavit_", "SHA1", 2, "Oas5!73m*a_G0dxi", 256);
        }
    }
    #endregion Класс Aes
}