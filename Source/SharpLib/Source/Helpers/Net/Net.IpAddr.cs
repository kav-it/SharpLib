using System;
using System.Xml.Serialization;

namespace SharpLib.Net
{
    /// <summary>
    /// Своя реализация представления IP-адреса (в .NET намудрили)
    /// </summary>
    public class NetIpAddr
    {
        #region Константы

        private const string LOCAL_HOST_AS_IP = "127.0.0.1";

        private const string LOCAL_HOST_AS_TEXT = "localhost";

        #endregion

        #region Свойства

        [XmlIgnore]
        public uint Value { get; set; }

        /// <summary>
        /// Текстовое представление адреса
        /// </summary>
        public string Text
        {
            get { return ToString(); }
            set { Value = ToIp(value); }
        }

        /// <summary>
        /// Константный адрес - localhost
        /// </summary>
        public static NetIpAddr Localhost
        {
            get { return new NetIpAddr(LOCAL_HOST_AS_TEXT); }
        }

        #endregion

        #region Конструктор

        public NetIpAddr()
        {
        }

        public NetIpAddr(string text)
        {
            Text = text;
        }

        public NetIpAddr(UInt32 ip)
        {
            Value = ip;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Перевод в текстовое представление
        /// </summary>
        public override string ToString()
        {
            return ToText(Value);
        }

        /// <summary>
        /// Преобразование в 4 байтовое значение
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static UInt32 ToIp(string text)
        {
            text = text.ToLower();
            text = text.Replace(',', '.');
            if (text == LOCAL_HOST_AS_TEXT)
            {
                text = LOCAL_HOST_AS_IP;
            }
            string[] arr = text.Split('.');
            if (arr.Length == 4)
            {
                int a = arr[0].ToIntEx();
                int b = arr[1].ToIntEx();
                int c = arr[2].ToIntEx();
                int d = arr[3].ToIntEx();

                return Pack(a, b, c, d);
            }

            return 0;
        }

        /// <summary>
        /// Преобразование в текстовое значение
        /// </summary>
        public static string ToText(UInt32 value)
        {
            byte a = (byte)(value >> 24);
            byte b = (byte)(value >> 16);
            byte c = (byte)(value >> 8);
            byte d = (byte)(value >> 0);

            string text = String.Format("{0}.{1}.{2}.{3}", a, b, c, d);

            return text;
        }

        /// <summary>
        /// Упаковка 4-х байт в IPv4
        /// </summary>
        public static UInt32 Pack(int a, int b, int c, int d)
        {
            UInt32 value = (((UInt32)a << 24) +
                            ((UInt32)b << 16) +
                            ((UInt32)c << 8) +
                            ((UInt32)d << 0));
            return value;
        }

        #endregion
    }
}