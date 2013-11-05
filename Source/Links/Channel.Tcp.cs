// ****************************************************************************
//
// Имя файла    : 'Channel.Tcp.cs'
// Заголовок    : Класс каналов TCP
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SharpLib
{
    public class ChannelTcp : LinkBase
    {
        #region Переменные
        private SocketClient _socketClient;
        #endregion Переменные

        #region Свойства
        public String Ip
        {
            get { return _socketClient.RemoteIp; }
            set { _socketClient.RemoteIp = value; }
        }
        public int Port
        {
            get { return _socketClient.RemotePort; }
            set { _socketClient.RemotePort = value; }
        }
        #endregion Свойства

        #region Конструктор
        public ChannelTcp() : base(ModuleTyp.ChannelTcp, "TCP")
        {
            _socketClient = new SocketClient();
            _socketClient.OnRead += new SocketReceiveHandler(SocketClientOnRead);
            _socketClient.OnDisconnect += new SocketBaseHandler(SocketClientOnDisconnect);
            _socketClient.OnBreak += new SocketBaseHandler(SocketClientOnBreak);
        }
        #endregion Конструктор

        #region Методы
        private void SocketClientOnRead(Socket winsock, Byte[] buffer)
        {
            // Трассировка входного буфера
            NotifyReceived(buffer);
            // Передача события
            RaiseDataReceived(buffer);
        }
        private void SocketClientOnBreak(object sender, Socket winsock)
        {
            NotifyClose(String.Format("Соединение {0} прервано", LinkName));

            RaiseBreak();
        }
        private void SocketClientOnDisconnect(object sender, Socket winsock)
        {
            NotifyClose(String.Format("Соединение {0} закрыто", LinkName));
        }
        protected override String GetLinkName()
        {
            String result = String.Format("{0}.{1}:{2}", ModName, Ip, Port);

            return result;
        }
        protected override Boolean OnOpen()
        {
            Boolean result = _socketClient.Open();

            if (result == true)
                NotifyOpen(String.Format("Соединение {0} установлено", LinkName));
            else
                NotifyError(String.Format("Ошибка: {0}", _socketClient.LastError));

            return result;
        }
        protected override Boolean OnClose()
        {
            _socketClient.Close();

            return true;
        }
        protected override Boolean OnSend(Byte[] buffer)
        {
            _socketClient.Send(buffer);

            return true;
        }
        #endregion Методы
    }
}
