﻿//*****************************************************************************
//
// Имя файла    : 'Stopwatch.Control.cs'
// Заголовок    : Компонент "Секундомер/Таймер"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 20/01/2013
//
//*****************************************************************************
			
using System;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib
{
    #region Перечисление
    public enum StopwatchControlTyp
    {
        Unknow,
        MMSS,
    }
    #endregion Перечисление

    #region Класс StopwatchControl
    public partial class StopwatchControl : UserControl
    {
        #region Конструктор
        public StopwatchControl()
        {
            InitializeComponent();
        }
        #endregion Конструктор

        #region Методы
        public void Start()
        {
        }
        public void Stop()
        {
        }
        public void Restart()
        {
        }
        #endregion Методы
    }
    #endregion Класс StopwatchControl
}

