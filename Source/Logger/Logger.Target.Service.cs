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
using System.ServiceModel;

using SharpLib.Wcf;

namespace SharpLib.Log
{

    #region Класс TargetService

    public class TargetService : Target
    {
        #region Свойства

        private LoggerService Service { get; set; }

        private LoggerConfigTargetService Config { get; set; }

        #endregion

        #region Конструктор

        public TargetService(LoggerConfigTargetService config) : base(TargetTyp.Service)
        {
            Service = new LoggerService();

            UpdateConfig(config);
        }

        #endregion

        #region Методы

        public void UpdateConfig(LoggerConfigTargetService config)
        {
            // Остановка сервиса
            Service.Stop();

            Config = config;

            // Запуск сервиса
            Service.StartPipe(Config.Address);
        }

        public override void Write(LogMessage message)
        {
            if (Service.IsOpened)
            {
                foreach (var callback in Service.CallbackList)
                {
                    CommunicationState state = ((ICommunicationObject)callback).State;

                    if (state == CommunicationState.Opened)
                        callback.OnMessage(message);
                }
            }
        }

        #endregion
    }

    #endregion Класс TargetService

    #region Интерфейс ILoggerService

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ILoggerCallback))]
    public interface ILoggerService : IWcfHost
    {
    }

    #endregion Интерфейс ILoggerService

    #region Интерфейс ILoggerCallback

    public interface ILoggerCallback : IWcfHostCallback
    {
        #region Методы

        [OperationContract(IsOneWay = true)]
        void OnMessage(LogMessage message);

        #endregion
    }

    #endregion Интерфейс ILoggerCallback

    #region Класс LoggerService

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LoggerService : WcfHost<ILoggerService, ILoggerCallback>, ILoggerService
    {
        #region Конструктор

        public LoggerService() : base("SharpLib.Logger")
        {
        }

        #endregion
    }

    #endregion Класс LoggerService

    public class LoggerServiceClient : WcfClient<ILoggerService>, ILoggerCallback
    {
        public void OnMessage(LogMessage message)
        {
            Console.WriteLine(message.ToString());
        }
    }
}