using System;
using System.Net;

namespace SharpLib.Net
{
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
            UInt16 port = (UInt16)ipEndPoint.Port;

            NetAddr result = new NetAddr(ip, port);

            return result;
        }

        #endregion
    }
}