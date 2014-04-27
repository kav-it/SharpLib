//*****************************************************************************
//
// Имя файла    : 'Diag.cs'
// Заголовок    : Модуль измерения производительности
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 28/04/2014
//
//*****************************************************************************

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpLib
{

    #region Класс Diag

    public static class Diag
    {
        #region Methods

        public static DiagTimer GetTimer()
        {
            return new DiagTimer();
        }

        #endregion
    }

    #endregion Класс Diag

    #region Класс DiagTimer

    public class DiagTimer
    {
        #region Fields

        private static readonly Int64 _freq;

        private Int64 _startTime;

        #endregion

        #region Constructors

        static DiagTimer()
        {
            QueryPerformanceFrequency(out _freq);
        }

        #endregion

        #region Properties

        public Int64 Ticks
        {
            get
            {
                Int64 time;
                QueryPerformanceCounter(out time);

                return time;
            }
        }

        public double Duration
        {
            get { return DiffTime(_startTime, Ticks); }
        }

        public string DurationText
        {
            get
            {
                double elapsedTime = Duration;

                string text = TimeToString(elapsedTime);

                return text;
            }
        }

        #endregion

        #region Methods

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out Int64 lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out Int64 lpFrequency);

        public void Start()
        {
            QueryPerformanceCounter(out _startTime);
        }

        private double DiffTime(Int64 beforeTime, Int64 afterTime)
        {
            double elapsed = (double)(afterTime - beforeTime) / _freq;

            return elapsed;
        }

        public string TimeToString(double value)
        {
            string dimension;

            if (value >= 1)
            {
                // Время в секундах
                dimension = " сек";
            }
            else
            {
                value = value * 1000;

                if (value >= 1)
                {
                    // Время в миллисекундах
                    dimension = " мс";
                }
                else
                {
                    // Время в миллисекундах
                    value = value * 1000;
                    dimension = " мкс";
                }
            }

            string result = value + dimension;

            return result;
        }

        public void Print()
        {
            // Вывод времени
            PrintText(DurationText + Environment.NewLine);
            // Перезапуск измерения (для удобства)
            Start();
        }

        private void PrintText(string value)
        {
            Debug.Write(value);
        }

        //public void PrintWithTime(string value)
        //{
        //    DateTime now = DateTime.Now;
        //    string time = now.ToStringEx();
        //    Print(time + value + Environment.NewLine);
        //}

        #endregion
    }

    #endregion Класс DiagTimer
}