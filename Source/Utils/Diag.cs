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
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpLib
{

    #region Класс Diag

    public static class Diag
    {
        #region Methods

        /// <summary>
        /// Создание таймера измерения времени с автозапуском
        /// </summary>
        public static DiagTimer StartTimer()
        {
            var timer = new DiagTimer();

            timer.Start();

            return timer;
        }

        /// <summary>
        /// Вывод в отладку Debug, но с добавление штампа времени
        /// </summary>
        public static void WriteLine(string format, params object[] args)
        {
            var text = string.Format(format, args);
            text = string.Format("[{0}] {1}", DateTime.Now.ToStringEx(), text);

            Debug.WriteLine(text);
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

        /// <summary>
        /// Значение длительности в 100 нс интервалах
        /// </summary>
        public double Duration
        {
            get { return DiffTime(_startTime, Ticks); }
        }

        /// <summary>
        /// Текстовое представление длительности
        /// </summary>
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

            string result = value.ToStringEx() + dimension;

            return result;
        }

        public void Print(string format, params object[] args)
        {
            string text = string.Format(format, args) + Environment.NewLine;

            // Вывод времени
            PrintText(text);
            // Перезапуск измерения (для удобства)
            Start();
        }

        private void PrintText(string value)
        {
            Debug.Write(value);
        }

        #endregion
    }

    #endregion Класс DiagTimer
}