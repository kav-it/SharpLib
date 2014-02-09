//*****************************************************************************
//
// Имя файла    : 'WaitIndicator.Control.cs'
// Заголовок    : Компонент "Индикатор ожидания"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 28/09/2012
//
//*****************************************************************************

using System;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib
{

    #region Класс BusyIndicatorControl

    public partial class BusyIndicatorControl : UserControl
    {
        #region Поля

        private Grid _grid;

        private Boolean _isVisible;

        private ContentControl _parent;

        private UIElement _parentContent;

        #endregion

        #region Конструктор

        public BusyIndicatorControl(ContentControl parent)
        {
            InitializeComponent();

            _isVisible = false;
            _parent = parent;
            _grid = new Grid();
        }

        #endregion

        #region Методы

        public void SetVisible(bool visible)
        {
            if (_isVisible != visible)
            {
                if (visible)
                {
                    // Сохранение контента
                    _parentContent = (UIElement)_parent.Content;

                    // Создание таблицы для инъекции в визуальное дерево
                    _grid = new Grid();
                    _parent.Content = _grid;
                    // Конфигурирование свойств
                    _parentContent.Opacity = 0.5;
                    _parentContent.IsEnabled = false;
                    // Инъекция таблицы
                    _grid.Children.Add(_parentContent);
                    _grid.Children.Add(this);
                }
                else
                {
                    _parentContent.Opacity = 1;
                    _parentContent.IsEnabled = true;
                    _grid.Children.Clear();
                    _parent.Content = _parentContent;
                }

                _isVisible = visible;
            }
        }

        public void Show()
        {
            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        #endregion
    }

    #endregion Класс BusyIndicatorControl
}