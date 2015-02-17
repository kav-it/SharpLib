using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Wpf.Dialogs
{
    internal sealed class DialogCustomHistory : NotifyModelBase
    {
        #region Поля

        /// <summary>
        /// Список путей "Вперед"
        /// </summary>
        private readonly Stack<string> _next;

        /// <summary>
        /// Список путей "Назад"
        /// </summary>
        private readonly Stack<string> _prev;

        /// <summary>
        /// Текущее положение
        /// </summary>
        private string _curr;

        /// <summary>
        /// Разрешен переход "Вперед"
        /// </summary>
        private bool _isEnableNext;

        /// <summary>
        /// Разрешен переход "Назад"
        /// </summary>
        private bool _isEnablePrev;

        /// <summary>
        /// Разрешен переход "Вверх"
        /// </summary>
        private bool _isEnableUp;

        #endregion

        #region Свойства

        /// <summary>
        /// Разрешен переход "Вперед"
        /// </summary>
        public bool IsEnableNext
        {
            get { return _isEnableNext; }
            set
            {
                _isEnableNext = value;
                RaisePropertyChanged("IsEnableNext");
            }
        }

        /// <summary>
        /// Разрешен переход "Назад"
        /// </summary>
        public bool IsEnablePrev
        {
            get { return _isEnablePrev; }
            set
            {
                _isEnablePrev = value;
                RaisePropertyChanged("IsEnablePrev");
            }
        }

        /// <summary>
        /// Разрешен переход "Назад"
        /// </summary>
        public bool IsEnableUp
        {
            get { return _isEnableUp; }
            set
            {
                _isEnableUp = value;
                RaisePropertyChanged("IsEnableUp");
            }
        }


        #endregion

        #region Конструктор

        internal DialogCustomHistory(string curr)
        {
            _prev = new Stack<string>();
            _next = new Stack<string>();
            _curr = curr;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обновление визуальных состояний модели
        /// </summary>
        private void UpdateValues()
        {
            IsEnablePrev = _prev.Any();
            IsEnableNext = _next.Any();
            IsEnableUp = Files.GetDirectoryParent(_curr).IsValid();
        }

        /// <summary>
        /// Извлечение предыдущего пути
        /// </summary>
        public string Prev()
        {
            if (_prev.Any())
            {
                var location = _prev.Pop();

                _next.Push(_curr);
                _curr = location;

                UpdateValues();

                return location;
            }

            return string.Empty;
        }

        /// <summary>
        /// Извлечение следующего пути
        /// </summary>
        public string Next()
        {
            if (_next.Any())
            {
                var location = _next.Pop();

                _prev.Push(_curr);
                _curr = location;

                UpdateValues();

                return location;
            }

            return string.Empty;
        }

        /// <summary>
        /// Добавление нового пути
        /// </summary>
        public void Add(string gotoLocation)
        {
            _prev.Push(_curr);
            _next.Clear();

            _curr = gotoLocation;

            UpdateValues();
        }

        #endregion
    }
}