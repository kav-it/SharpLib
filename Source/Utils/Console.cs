//*****************************************************************************
//
// Имя файла    : 'Console.cs'
// Заголовок    : Модуль-помощник для реализации работы с консолью
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 20/12/2012
//
//*****************************************************************************

using System;

namespace SharpLib
{
    #region Класс Consoler
    public class Consoler
    {
        #region Методы
        public static void Print(String text, ConsoleColor color = ConsoleColor.White)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = currentColor;
        }
        public static void NextLine()
        {
            Print("\r\n");
        }
        public static ConsoleKey WaitPressKey(String text = "Нажмите любую клавишу...")
        {
            Print(text);

            // Ожидание нажатия клавиши
            ConsoleKeyInfo info = Console.ReadKey(true);
                
            return info.Key;
        }
        #endregion Методы
    }
    #endregion Класс Consoler
}