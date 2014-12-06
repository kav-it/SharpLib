using System;
using System.Collections.Generic;

namespace SharpLib.Net
{
    /// <summary>
    /// Серверный сокет
    /// </summary>
    public class NetSocketServer : IDisposable
    {
        #region Поля

        private readonly NetSocket _listenSocket;

        #endregion

        #region Свойства

        /// <summary>
        /// Список сокетов, установивших соединение
        /// </summary>
        public List<NetSocket> Sockets { get; private set; }

        #endregion

        #region События

        /// <summary>
        /// Установлено соединение
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnConnect;

        /// <summary>
        /// Разорвано соединение (инициатива клиента)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnDisconnect;

        /// <summary>
        /// Разорвано соединение (инициатива сервера)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnBreak;

        /// <summary>
        /// Сокет переведен в режим приема входящих соединений
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnListen;

        /// <summary>
        /// Сокет закрыт
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnClose;

        /// <summary>
        /// Приняты данные от удаленной точки
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnReceive;

        #endregion

        #region Конструктор

        public NetSocketServer()
        {
            OnListen = null;
            OnClose = null;
            OnConnect = null;
            OnReceive = null;

            _listenSocket = new NetSocket();
            _listenSocket.OnAccept += SocketOnAccept;
            _listenSocket.OnListen += SocketOnListen;
            _listenSocket.OnClose += SocketOnClose;

            Sockets = new List<NetSocket>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка СЕРВЕРНОГО сокета "Сокет переведен в режим приема соединений"
        /// </summary>
        private void SocketOnListen(object sender, NetSocketEventArgs args)
        {
            if (OnListen != null)
            {
                OnListen(sender, args);
            }
        }

        /// <summary>
        /// Обработка СЕРВЕРНОГО сокета "Сокет закрыт"
        /// </summary>
        private void SocketOnClose(object sender, NetSocketEventArgs args)
        {
            if (OnClose != null)
            {
                OnClose(sender, args);
            }
        }

        /// <summary>
        /// Обработка СЕРВЕРНОГО сокета "Установлено входящее соединение"
        /// </summary>
        private void SocketOnAccept(object sender, NetSocketEventArgs args)
        {
            // Создание нового объекта SharpLib-сокета, использующего указанный .NET-сокет
            var newSocket = new NetSocket(args.Sock);
            newSocket.OnBreak += SocketOnDisconnect;
            newSocket.OnReceive += SocketOnReceive;
            newSocket.OnDisconnect += SocketOnBreak;

            // Добавление в список нового сокета
            Sockets.Add(newSocket);
            // Перевод сокета в режим приема данных
            newSocket.Receive();

            // Генерация события
            if (OnConnect != null)
            {
                // Передача события приложению
                OnConnect(newSocket, args);
            }
        }

        /// <summary>
        /// Обработка КЛИЕНТСКОГО сокета "Получены данные"
        /// </summary>
        private void SocketOnReceive(object sender, NetSocketEventArgs args)
        {
            if (OnReceive != null)
            {
                OnReceive(sender, args);
            }
        }

        /// <summary>
        /// Обработка КЛИЕНТСКОГО сокета "Соединение разорвано (по инициативе удаленной точки)"
        /// </summary>
        private void SocketOnDisconnect(object sender, NetSocketEventArgs args)
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(sender, args);
            }

            // Удаление сокета из списка
            var socket = sender as NetSocket;
            Sockets.Remove(socket);
        }

        /// <summary>
        /// Обработка КЛИЕНТСКОГО сокета "Соединение разорвано (по инициативе сервера)"
        /// </summary>
        private void SocketOnBreak(object sender, NetSocketEventArgs args)
        {
            if (OnBreak != null)
            {
                OnBreak(sender, args);
            }
        }

        /// <summary>
        /// Перевод сервера в режим приема входящих соединений
        /// </summary>
        public NetSocketError Listen(int port)
        {
            var error = _listenSocket.Listen(port);

            return error;
        }

        /// <summary>
        /// Закрытие сокета
        /// </summary>
        public void Close()
        {
            // Закрытие текущих клиентских соединений 
            foreach (var socket in Sockets)
            {
                socket.Close();
            }

            Sockets.Clear();

            // Закрытие текущего соединения
            _listenSocket.Close();
        }

        /// <summary>
        /// Освобождение managed-ресурсов
        /// </summary>
        public void Dispose()
        {
            foreach (var socket in Sockets)
            {
                socket.Dispose();
            }

            _listenSocket.Dispose();
        }

        #endregion
    }
}