using System;

namespace SharpLib.Docking.Controls
{
    internal class ReentrantFlag
    {
        #region Поля

        private bool _flag;

        #endregion

        #region Свойства

        public bool CanEnter
        {
            get { return !_flag; }
        }

        #endregion

        #region Методы

        public ReentrantFlagHandler Enter()
        {
            if (_flag)
            {
                throw new InvalidOperationException();
            }
            return new ReentrantFlagHandler(this);
        }

        #endregion

        #region Вложенный класс: _ReentrantFlagHandler

        public class ReentrantFlagHandler : IDisposable
        {
            #region Поля

            private readonly ReentrantFlag _owner;

            #endregion

            #region Конструктор

            public ReentrantFlagHandler(ReentrantFlag owner)
            {
                _owner = owner;
                _owner._flag = true;
            }

            #endregion

            #region Методы

            public void Dispose()
            {
                _owner._flag = false;
            }

            #endregion
        }

        #endregion
    }
}