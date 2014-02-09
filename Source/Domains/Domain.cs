// ****************************************************************************
//
// Имя файла    : 'File.cs'
// Заголовок    : Это файл предназначен для ...
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 20/06/2012
//
// ****************************************************************************

using System;

namespace SharpLib
{
    public class AppDomainEx
    {
        #region Свойства

        public static AppDomain Current
        {
            get { return AppDomain.CurrentDomain; }
        }

        public AppDomain Value { private get; set; }

        #endregion

        #region События

        public event EventHandler<EventArgs> DomainUnload;

        public event EventHandler<EventArgs> ProcessExit;

        #endregion

        #region Конструктор

        public AppDomainEx() : this(Current)
        {
        }

        public AppDomainEx(AppDomain appDomain)
        {
            Value = appDomain;
            Value.ProcessExit += OnProcessExit;
            Value.DomainUnload += OnDomainUnload;
        }

        #endregion

        #region Методы

        private void OnDomainUnload(object sender, EventArgs e)
        {
            if (DomainUnload != null)
                DomainUnload(sender, e);
        }

        private void OnProcessExit(object sender, EventArgs eventArgs)
        {
            if (ProcessExit != null)
                ProcessExit(sender, eventArgs);
        }

        #endregion
    }
}