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

        public static void DeepCopy(Object target, Object source)
        {
            Type sourceType = source.GetType();

            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite) continue;

                if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType == typeof(String))
                {
                    property.SetValueEx(target, property.GetValueEx(source));
                }
                else
                {
                    Object propertyValue = property.GetValueEx(source);

                    if (propertyValue == null)
                    {
                        property.SetValueEx(target, null);
                    }
                    else
                    {
                        // Создание объекта по типу
                        var value = CreateObject(propertyValue.GetType());
                        // Копирование данных объекта
                        DeepCopy(value, propertyValue);
                        // Инициализация свойства созданным значением
                        property.SetValueEx(target, value);
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
















