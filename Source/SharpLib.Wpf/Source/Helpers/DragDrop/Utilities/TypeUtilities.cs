using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SharpLib.Wpf.Dragging.Utilities
{
    internal static class TypeUtilities
    {
        #region Методы

        public static IEnumerable CreateDynamicallyTypedList(IEnumerable source)
        {
            var enumerable = source as IList<object> ?? Enumerable.ToList(source.Cast<object>());
            var type = GetCommonBaseClass(enumerable);
            var listType = typeof(List<>).MakeGenericType(type);
            var addMethod = listType.GetMethod("Add");
            var constructorInfo = listType.GetConstructor(Type.EmptyTypes);
            if (constructorInfo != null)
            {
                var list = constructorInfo.Invoke(null);

                foreach (var o in enumerable)
                {
                    addMethod.Invoke(list, new[] { o });
                }

                return (IEnumerable)list;
            }

            return null;
        }

        public static Type GetCommonBaseClass(IEnumerable e)
        {
            var types = e.Cast<object>().Select(o => o.GetType()).ToArray();
            return GetCommonBaseClass(types);
        }

        public static Type GetCommonBaseClass(Type[] types)
        {
            if (types.Length == 0)
            {
                return typeof(object);
            }

            var ret = types[0];

            for (var i = 1; i < types.Length; ++i)
            {
                if (types[i].IsAssignableFrom(ret))
                {
                    ret = types[i];
                }
                else
                {
                    while (ret != null && !ret.IsAssignableFrom(types[i]))
                    {
                        ret = ret.BaseType;
                    }
                }
            }

            return ret;
        }

        public static IList ToList(this IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return ((ICollectionView)enumerable).SourceCollection as IList;
            }
            return enumerable as IList;
        }

        #endregion
    }
}