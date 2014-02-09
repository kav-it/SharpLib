//*****************************************************************************
//
// Имя файла    : 'Core.Net.cs'
// Заголовок    : Подсистема сокетов на базе SharpLib
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/04/2013
//
//*****************************************************************************

using System.Collections.Generic;
using System.Net.Sockets;

namespace SharpLib
{

    #region Класс NetSocketServer

    public class NetSocketServer
    {
        #region Поля

        private NetSocket _listenSocket;

        #endregion

        #region Свойства

        public List<NetSocket> Sockets { get; private set; }

        #endregion

        #region События

        public event NetSocketNotifyEvent OnAccept;
        public event NetSocketNotifyEvent OnBreak;
        public event NetSocketNotifyEvent OnClose;
        public event NetSocketNotifyEvent OnListen;
        public event NetSocketReadEvent OnReceive;

        #endregion

        #region Конструктор

        public NetSocketServer()
        {
            OnListen = null;
            OnClose = null;
            OnAccept = null;
            OnReceive = null;

            _listenSocket = new NetSocket();
            _listenSocket.OnAccept += SocketOnAccept;
            _listenSocket.OnListen += SocketOnListen;
            _listenSocket.OnClose += SocketOnClose;

            Sockets = new List<NetSocket>();
        }

        #endregion

        #region Обработка событий

        private void SocketOnListen(NetSocket sender, Socket sock)
        {
            // Генерация события
            if (OnListen != null)
                OnListen(sender, sock);
        }

        private void SocketOnClose(NetSocket sender, Socket sock)
        {
            // Генерация события
            if (OnClose != null)
                OnClose(sender, sock);
        }

        private void SocketOnAccept(NetSocket sender, Socket sock)
        {
            NetSocket newSocket = new NetSocket(sock);
            newSocket.OnBreak += SocketOnBreak;
            newSocket.OnReceive += SocketOnReceive;

            // Добавление в список нового сокета
            Sockets.Add(newSocket);
            // Перевод сокета в режим приема данных
            newSocket.Receive();

            // Генерация события
            if (OnAccept != null)
            {
                // Передача события приложению
                OnAccept(newSocket, sock);
            }
        }

        private void SocketOnReceive(NetSocket sender, Socket sock, byte[] buffer)
        {
            if (OnReceive != null)
                OnReceive(sender, sock, buffer);
        }

        private void SocketOnBreak(NetSocket sender, Socket sock)
        {
            if (OnBreak != null)
                OnBreak(sender, sock);

            // Удаление сокета из списка
            Sockets.Remove(sender);
        }

        #endregion Обработка событий

        #region Управление

        public NetSocketError Listen(int port)
        {
            NetSocketError error = _listenSocket.Listen(port);

            return error;
        }

        public void Close()
        {
            // Закрытие текущих клиентских соединений 
            foreach (NetSocket socket in Sockets)
                socket.Disconnect();

            // Закрытие текущего соединения
            _listenSocket.Close();
        }

        #endregion Управление
    }

    #endregion Класс NetSocketServer
}