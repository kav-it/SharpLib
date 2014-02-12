// ****************************************************************************
//
// Имя файла    : 'Reflector.cs'
// Заголовок    : Класс для работы с рефлексией в коде (run-time)
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 02/02/2014
//
// ****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SharpLib
{

    #region Класс Reflector

    public class Reflector
    {
        #region Методы

        public static Object CreateObject(Type typ)
        {
            Object result = Activator.CreateInstance(typ);

            return result;
        }

        private static Boolean IsSimplyTyp(Type typ)
        {
            Boolean result = typ.IsValueType || typ.IsEnum || typ == typeof(String);

            return result;
        }

        public static void DeepCopy(Object dest, Object source)
        {
            Type destType = source.GetType();
            Type sourceType = source.GetType();

            if (destType != sourceType) return;

            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite) continue;

                Object propertyValue = property.GetValueEx(source);

                // Проверка, что тип является:
                //  + значимым типом
                //  + перечисление (тоже значимый тип, но отдельная обработка)
                //  + строка (отдельная обработка)
                //  + значение свойства == null
                if (IsSimplyTyp(property.PropertyType) || propertyValue == null)
                {
                    // Установка свойства объекта "Dest"
                    property.SetValueEx(dest, propertyValue);
                }
                else
                {
                    IList list = propertyValue as IList;

                    if (list != null)
                    {
                        // Создание объекта по типу
                        var value = Reflector.CreateObject(propertyValue.GetType());

                        foreach (Object listItem in list)
                        {
                            Type listItemTyp = listItem.GetType();
                            Object destItem;

                            // Прямое копирование значимых типов и строк
                            if (IsSimplyTyp(listItemTyp))
                            {
                                // Прямое копирование
                                destItem = listItem;
                            }
                            else
                            {
                                // Создание элемента списка по типу
                                destItem = Reflector.CreateObject(listItemTyp);
                                // Копирование данных объекта (через рефлексию)
                                DeepCopy(destItem, listItem);
                            }
                            // Добавление нового свойства в список
                            ((IList)value).Add(destItem);
                        }

                        // Установка свойства объекта "Dest"
                        property.SetValueEx(dest, value);
                    }
                    else
                    {
                        // Создание объекта по типу
                        var value = Reflector.CreateObject(propertyValue.GetType());
                        // Копирование данных объекта
                        DeepCopy(value, propertyValue);
                        // Установка свойства объекта "Dest"
                        property.SetValueEx(dest, value);
                    }
                }
            }
        }

        #endregion
    }

    #endregion Класс Reflector

    #region Класс ExtensionReflector
    public static class ExtensionReflector
    {
        public static Object GetValueEx(this PropertyInfo property, Object obj)
        {
            #if __NET35__ || __NET40__
            return property.GetValue(obj, null);    
            #else
            return property.GetValue(obj);    
            #endif
        }

        public static void SetValueEx(this PropertyInfo property, Object obj, Object value)
        {
            #if __NET35__ || __NET40__
            property.SetValue(obj, value, null);    
            #else
            property.SetValue(obj, value);    
            #endif
        }
    }
    #endregion Класс ExtensionReflector
}
















