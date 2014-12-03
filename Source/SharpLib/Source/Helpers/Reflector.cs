using System;
using System.Collections;
using System.Reflection;

namespace SharpLib
{
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

        #endregion
    }
}