using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpLib.Log
{
    internal class MethodFactory<TClassAttributeType, TMethodAttributeType> : INamedItemFactory<MethodInfo, MethodInfo>, IFactory
        where TClassAttributeType : Attribute
        where TMethodAttributeType : NameBaseAttribute
    {
        #region Поля

        private readonly Dictionary<string, MethodInfo> nameToMethodInfo = new Dictionary<string, MethodInfo>();

        #endregion

        #region Свойства

        public IDictionary<string, MethodInfo> AllRegisteredItems
        {
            get { return nameToMethodInfo; }
        }

        #endregion

        #region Методы

        public void ScanTypes(Type[] types, string prefix)
        {
            foreach (Type t in types)
            {
                try
                {
                    RegisterType(t, prefix);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }
                }
            }
        }

        public void RegisterType(Type type, string itemNamePrefix)
        {
            if (type.IsDefined(typeof(TClassAttributeType), false))
            {
                foreach (MethodInfo mi in type.GetMethods())
                {
                    var methodAttributes = (TMethodAttributeType[])mi.GetCustomAttributes(typeof(TMethodAttributeType), false);
                    foreach (TMethodAttributeType attr in methodAttributes)
                    {
                        RegisterDefinition(itemNamePrefix + attr.Name, mi);
                    }
                }
            }
        }

        public void Clear()
        {
            nameToMethodInfo.Clear();
        }

        public void RegisterDefinition(string name, MethodInfo methodInfo)
        {
            nameToMethodInfo[name] = methodInfo;
        }

        public bool TryCreateInstance(string name, out MethodInfo result)
        {
            return nameToMethodInfo.TryGetValue(name, out result);
        }

        public MethodInfo CreateInstance(string name)
        {
            MethodInfo result;

            if (TryCreateInstance(name, out result))
            {
                return result;
            }

            throw new Exception("Unknown function: '" + name + "'");
        }

        public bool TryGetDefinition(string name, out MethodInfo result)
        {
            return nameToMethodInfo.TryGetValue(name, out result);
        }

        #endregion
    }
}