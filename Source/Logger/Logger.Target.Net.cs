// ****************************************************************************
//
// Имя файла    : 'Logger.cs'
// Заголовок    : Реализация записи лог-файлов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;

namespace SharpLib.Log
{

    #region Класс TargetNet

    public class TargetNet : Target
    {
        #region Свойства

        private LoggerConfigTargetNet Config { get; set; }

        private NetSocket Socket { get; set; }

        #endregion

        #region Конструктор

        public TargetNet(LoggerConfigTargetNet config) : base(TargetTyp.Net)
        {
            Config = config;

            UpdateConfig(config);
        }

        #endregion

        #region Методы

        private void UpdateConfig(LoggerConfigTargetNet config)
        {
            Config = config;

            Open();
        }

        private void Open()
        {
            String uri = Config.Uri.Replace(@"\\", @"//");

            // Разбор строки по формату: [proto]:\\[ip]:[port]
            if (uri.StartsWith("udp://"))
            {
                uri = uri.Remove("udp://");

                NetAddr addr = new NetAddr(uri);
                Socket = new NetSocket(NetProto.Udp);
                Socket.Connect(addr);
            }
            else
                throw new NotImplementedException();
        }

        public override void Write(LogMessage message)
        {
            Byte[] buffer = message.ToBuffer();

            Socket.SendBuffer(buffer);
        }

        #endregion
    }

    #endregion Класс TargetNet
}