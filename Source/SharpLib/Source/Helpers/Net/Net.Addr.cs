using System;

namespace SharpLib
{
    /// <summary>
    /// Своя реализация представления сетевого адреса (IP + Port) (в .NET намудрили)
    /// </summary>
    public class NetAddr
    {
        #region Свойства

        /// <summary>
        /// IP адрес
        /// </summary>
        public NetIpAddr Ip { get; set; }

        /// <summary>
        /// Порт
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region Конструктор

        public NetAddr()
        {
            Ip = NetIpAddr.Localhost;
            Port = 0;
        }

        public NetAddr(int port)
            : this()
        {
            Port = port;
        }

        public NetAddr(UInt32 ip, int port)
        {
            Ip = new NetIpAddr(ip);
            Port = port;
        }

        public NetAddr(String text)
        {
            Parse(text);
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return ToString(Ip.Value, Port);
        }

        public static String ToString(UInt32 ip, int port)
        {
            string result = string.Format("{0}:{1}", NetIpAddr.ToText(ip), port);

            return result;
        }

        private void Parse(string value)
        {
            string[] blocks = value.Split(':');
            if (blocks.Length != 2)
            {
                return;
            }

            Ip = new NetIpAddr(blocks[0]);
            Port = blocks[1].ToIntEx();
        }

        #endregion
    }
}