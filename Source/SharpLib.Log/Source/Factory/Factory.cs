using System;
using System.Collections.Generic;

namespace SharpLib.Log
{
    internal class Factory<TBaseType, TAttributeType> : INamedItemFactory<TBaseType, Type>, IFactory
        where TBaseType : class
        where TAttributeType : NameBaseAttribute
    {
        #region Делегаты

        private delegate Type GetTypeDelegate();

        #endregion

        #region Поля

        private readonly Dictionary<string, GetTypeDelegate> items = new Dictionary<string, GetTypeDelegate>(StringComparer.OrdinalIgnoreCase);

        private readonly ConfigurationItemFactory parentFactory;

        #endregion

        #region Конструктор

        internal Factory(ConfigurationItemFactory parentFactory)
        {
            this.parentFactory = parentFactory;
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
            TAttributeType[] attributes = (TAttributeType[])type.GetCustomAttributes(typeof(TAttributeType), false);
            if (attributes != null)
            {
                foreach (TAttributeType attr in attributes)
                {
                    RegisterDefinition(itemNamePrefix + attr.Name, type);
                }
            }
        }

        public void RegisterNamedType(string itemName, string typeName)
        {
            items[itemName] = () => Type.GetType(typeName, false);
        }

        public void Clear()
        {
            items.Clear();
        }

        public void RegisterDefinition(string name, Type type)
        {
            items[name] = () => type;
        }

        public bool TryGetDefinition(string itemName, out Type result)
        {
            GetTypeDelegate del;

            if (!items.TryGetValue(itemName, out del))
            {
                result = null;
                return false;
            }

            try
            {
                result = del();
                return result != null;
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                {
                    throw;
                }

                result = null;
                return false;
            }
        }

        public bool TryCreateInstance(string itemName, out TBaseType result)
        {
            Type type;

            if (!TryGetDefinition(itemName, out type))
            {
                result = null;
                return false;
            }

            result = (TBaseType)parentFactory.CreateInstance(type);
            return true;
        }

        public TBaseType CreateInstance(string name)
        {
            TBaseType result;

            if (TryCreateInstance(name, out result))
            {
                return result;
            }

            throw new ArgumentException(typeof(TBaseType).Name + " cannot be found: '" + name + "'");
        }

        #endregion
    }
}