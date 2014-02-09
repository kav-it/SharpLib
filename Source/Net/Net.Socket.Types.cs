//*****************************************************************************
//
// Имя файла    : 'Net.Types.cs'
// Заголовок    : Подсистема сокетов на базе SharpLib
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/04/2013
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace SharpLib
{
    #region Делегат NetSocketEvent

    public delegate void NetSocketNotifyEvent(NetSocket sender, Socket sock);

    public delegate void NetSocketReadEvent(NetSocket sender, Socket sock, Byte[] buffer);

    public delegate void NetSocketErrorEvent(NetSocket sender, Socket sock, SocketError error);

    #endregion Делегат NetSocketTyp

    #region Перечисление NetSocketState

    public enum NetSocketState
    {
        Unknow = 0,
        Opened = 1,
        Closed = 2,
        Listen = 3,
        Opening = 4,
        Closing = 5
    }

    #endregion Перечисление NetSocketState

    #region Перечисление NetSocketError
    public enum NetSocketError
    {
        [Description("Ошибка")]
        Unknow      = 0,
        [Description("")]
        Ok          = 1,
        [Description("Порт занят")]
        Busy        = 2
    }
    #endregion Перечисление NetSocketError

    #region Класс NetIpAddr

    public class NetIpAddr
    {
        #region Поля

        private UInt32 _value;

        #endregion

        #region Свойства

        [XmlIgnore]
        public UInt32 Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public String Text
        {
            get { return ToString(); }
            set { _value = ToIp(value); }
        }

        public static NetIpAddr Localhost
        {
            get { return new NetIpAddr("localhost"); }
        }

        #endregion

        #region Конструктор

        public NetIpAddr()
        {
        }

        public NetIpAddr(String text)
        {
            Text = text;
        }

        public NetIpAddr(UInt32 ip)
        {
            Value = ip;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return ToText(Value);
        }

        public static UInt32 ToIp(String text)
        {
            text = text.ToLower();
            text = text.Replace(',', '.');
            if (text == "localhost") text = "127.0.0.1";
            String[] arr = text.Split('.');
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

        public static String ToText(UInt32 value)
        {
            Byte a = (Byte) (value >> 24);
            Byte b = (Byte) (value >> 16);
            Byte c = (Byte) (value >> 8);
            Byte d = (Byte) (value >> 0);

            String text = String.Format("{0}.{1}.{2}.{3}", a, b, c, d);

            return text;
        }

        public static UInt32 Pack(int a, int b, int c, int d)
        {
            UInt32 value = (((UInt32) a << 24) +
                            ((UInt32) b << 16) +
                            ((UInt32) c << 8) +
                            ((UInt32) d << 0));
            return value;
        }

        #endregion
    }

    #endregion Класс NetIpAddr

    #region Класс NetAddr

    public class NetAddr
    {
        #region Свойства

        public NetIpAddr Ip { get; set; }

        public int Port { get; set; }

        #endregion

        #region Конструктор

        public NetAddr()
        {
            Ip = NetIpAddr.Localhost;
            Port = 0;
        }

        public NetAddr(int port) : this()
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

        public override String ToString()
        {
            return NetAddr.ToString(Ip.Value, Port);
        }

        public static String ToString(UInt32 ip, int port)
        {
            String result = String.Format("{0}:{1}", NetIpAddr.ToText(ip), port);

            return result;
        }

        private void Parse(String value)
        {
            String[] blocks = value.Split(':');
            if (blocks.Length != 2) return;

            Ip   = new NetIpAddr(blocks[0]);
            Port = blocks[1].ToIntEx();
        }

        #endregion
    }

    #endregion Класс NetAddr

    #region Класс NetExtension

    public static class NetExtension
    {
        #region Методы

        public static UInt32 ToIpv4Ex(this IPAddress value)
        {
            Byte[] buf = value.GetAddressBytes();
            UInt32 result = buf.GetByte32Ex(0, Endianess.Big);

            return result;
        }

        public static EndPoint ToEndPointEx(this NetAddr value)
        {
            IPAddress ipAddr = new IPAddress(value.Ip.Value.SwitchOrderEx());
            IPEndPoint result = new IPEndPoint(ipAddr, value.Port);

            return result;
        }

        public static NetAddr ToNetAddrEx(this EndPoint value)
        {
            IPEndPoint ipEndPoint = value as IPEndPoint;
            UInt32 ip = ipEndPoint.Address.ToIpv4Ex();
            UInt16 port = (UInt16) ipEndPoint.Port;

            NetAddr result = new NetAddr(ip, port);

            return result;
        }

        #endregion
    }

    #endregion Класс NetExtension
}