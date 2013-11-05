// ****************************************************************************
//
// Имя файла    : 'Attributes.cs'
// Заголовок    : Модуль работы с аттрибутами
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 31/08/2012
//
// ****************************************************************************

using System;
using System.Reflection;

namespace SharpLib
{
    #region Класс EnumTextAttribute
    /// <summary>
    /// Аттрибут для текстового преобразования Enum
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal sealed class EnumTextAttribute : Attribute
    {
        #region Поля
        private  String _text;
        #endregion Поля

        #region Свойства
        public String Text
        {
            get { return _text; }
        }
        #endregion Свойства

        #region Конструктор
        public EnumTextAttribute(String text)
        {
            _text = text;
        }
        #endregion Конструктор
    }
    #endregion Класс EnumTextAttribute

    #region Класс Attributes
    public static class Attributes
    {
        #region Методы
        /// <summary>
        /// Вывод в отладку текущих аттрибутов объекта
        /// </summary>
        /// <param name="obj"></param>
        public static void Print (Object obj)
        {
            Type   type = obj.GetType();
            String name = obj.ToString();

            // Вывод аттрибутов класса
            foreach (Attribute attr in type.GetCustomAttributes(true))
            {
                Console.WriteLine("Аттрибут объекта {0}: {1}", name, attr.ToString());
            }

            // Вывод аттрибутов методов класса
            foreach (MethodInfo method in type.GetMethods())
            {
                foreach (Attribute attr in method.GetCustomAttributes(true))
                {
                    Console.WriteLine("Аттрибут метода {0}: {1}", method.Name, attr.ToString());
                }
            }

            // Вывод аттрибутов полей класса (только public)
            foreach (FieldInfo field in type.GetFields())
            {
                foreach (Attribute attr in field.GetCustomAttributes(true))
                {
                    Console.WriteLine("Аттрибут поля {0}: {1}", field.Name, attr.ToString());
                }
            }
        }
        /// <summary>
        /// Преобразование Enum в строку на основе аттрибута EnumTextAttribute
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToText(this Enum value)
        {
            Type   type = value.GetType();
            String name = Enum.GetName(type, value);

            if (name != null)
            {
                FieldInfo field = type.GetField(name);

                if (field != null)
                {
                    if (field.IsDefined(typeof(EnumTextAttribute), false))
                    {
                        EnumTextAttribute attr = (EnumTextAttribute)Attribute.GetCustomAttribute(field, typeof(EnumTextAttribute));

                        return attr.Text;
                    }
                }
            }

            return value.ToString();
        }
        #endregion Методы
    }
    #endregion Класс Attributes
}
