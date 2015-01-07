using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpLib
{
    /// <summary>
    /// Класс работы с отражением
    /// </summary>
    public class Reflector
    {
        #region Методы

        /// <summary>
        /// Создание объекта в run-time
        /// </summary>
        public static object CreateObject(Type typ)
        {
            var result = Activator.CreateInstance(typ);

            return result;
        }

        /// <summary>
        /// Проверка является ли тип "простым" (используется в DeepCopy)
        /// </summary>
        private static bool IsSimplyTyp(Type typ)
        {
            bool result = typ.IsValueType || typ.IsEnum || typ == typeof(string);

            return result;
        }

        /// <summary>
        /// Глубокое копирование объекта
        /// </summary>
        public static void DeepCopy(Object dest, Object source)
        {
            Type destType = source.GetType();
            Type sourceType = source.GetType();

            if (destType != sourceType)
            {
                return;
            }

            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                Object propertyValue = property.GetValueEx(source);

                if (IsSimplyTyp(property.PropertyType) || propertyValue == null)
                {
                    property.SetValueEx(dest, propertyValue);
                }
                else
                {
                    IList list = propertyValue as IList;

                    if (list != null)
                    {
                        var value = CreateObject(propertyValue.GetType());

                        foreach (Object listItem in list)
                        {
                            Type listItemTyp = listItem.GetType();
                            Object destItem;

                            if (IsSimplyTyp(listItemTyp))
                            {
                                destItem = listItem;
                            }
                            else
                            {
                                destItem = CreateObject(listItemTyp);

                                DeepCopy(destItem, listItem);
                            }

                            ((IList)value).Add(destItem);
                        }

                        property.SetValueEx(dest, value);
                    }
                    else
                    {
                        var value = CreateObject(propertyValue.GetType());

                        DeepCopy(value, propertyValue);

                        property.SetValueEx(dest, value);
                    }
                }
            }
        }

        /// <summary>
        /// Чтение типов из указанной сборки без загрузки в домен
        /// </summary>
        public static List<Type> GetTypesReflectionOnly(string location)
        {
            using (var loader = new ReflectionOnlyLoader(location))
            {
                var asm = loader.Load();

                if (asm != null)
                {
                    return asm.GetTypes().ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Чтение значений вложенных свойств
        /// </summary>
        /// <remarks>
        /// Пример формата: "Property1.Property2.A"
        /// </remarks>
        public static object GetPropertyNestedEx(object obj, string propertyName)
        {
            // Проверки
            if (obj == null || propertyName.IsNotValid())
            {
                return null;
            }

            // Разделение запрашиваемого пути свойства
            var names = propertyName.Split('.');

            foreach (var name in names)
            {
                var objType = obj.GetType();

                if (objType == typeof(ExpandoObject))
                {
                    // Динамический объект ExpandoObject является словарем, 
                    // поэтому первое свойство берется из словаря
                    var dictionary = (IDictionary<string, object>)obj;
                    obj = dictionary.GetValueEx(name);
                }
                else
                {
                    var propInfo = objType.GetProperty(name);
                    if (propInfo == null)
                    {
                        return null;
                    }
                    obj = propInfo.GetValueEx(obj);
                }

                if (obj == null)
                {
                    return null;
                }
            }

            return obj;
        }

        #endregion

        #region Вложенный класс: ReflectionOnlyLoader

        /// <summary>
        /// Класс загрузки сборок только для Reflection
        /// </summary>
        private class ReflectionOnlyLoader : IDisposable
        {
            #region Поля

            private readonly string _location;

            #endregion

            #region Конструктор

            internal ReflectionOnlyLoader(string location)
            {
                _location = location;

                // Установка обработчика разрешения зависимостей при предварительной загрузке
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;
            }

            #endregion

            #region Методы

            private Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
            {
                var name = new AssemblyName(args.Name);
                var root = Path.GetDirectoryName(_location);

                if (root != null)
                {
                    var asmToCheck = Path.Combine(root, name.Name) + ".dll";
                    if (File.Exists(asmToCheck))
                    {
                        return Assembly.ReflectionOnlyLoadFrom(asmToCheck);
                    }
                }

                return Assembly.ReflectionOnlyLoad(args.Name);
            }

            internal Assembly Load()
            {
                try
                {
                    return Assembly.ReflectionOnlyLoadFrom(_location);
                }
                catch
                {
                    return null;
                }
            }

            public void Dispose()
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= ReflectionOnlyAssemblyResolve;
            }

            #endregion
        }

        #endregion
    }
}