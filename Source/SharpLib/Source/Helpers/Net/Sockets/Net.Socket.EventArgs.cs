using System;
using System.Net.Sockets;

namespace SharpLib.Net
{
    /// <summary>
    /// Событие сокетов
    /// </summary>
    public class NetSocketEventArgs : EventArgs
    {
        #region Свойства

        /// <summary>
        /// .NET сокет (для дополнительной информации об операции)
        /// </summary>
        public Socket Sock { get; private set; }

        /// <summary>
        /// Данные (заполняются при событии OnReceived)
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Код ошибки (заполняется при событии OnError)
        /// </summary>
        public SocketError Error { get; private set; }

        #endregion

        #region Конструктор

        public NetSocketEventArgs(Socket sock) : this(sock, SocketError.Success, null)
        {
        }

        public NetSocketEventArgs(Socket sock, SocketError error, byte[] buffer)
        {
            Sock = sock;
            Error = error;
            Buffer = buffer;
        }

        #endregion
    }
}