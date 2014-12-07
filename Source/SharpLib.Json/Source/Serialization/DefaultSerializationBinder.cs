using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace SharpLib.Json
{
    public class DefaultSerializationBinder : SerializationBinder
    {
        #region Поля

        internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();

        private readonly ThreadSafeStore<TypeNameKey, Type> _typeCache = new ThreadSafeStore<TypeNameKey, Type>(GetTypeFromTypeNameKey);

        #endregion

        #region Методы

        private static Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey)
        {
            string assemblyName = typeNameKey.AssemblyName;
            string typeName = typeNameKey.TypeName;

            if (assemblyName != null)
            {
#pragma warning disable 618,612
                Assembly assembly = Assembly.LoadWithPartialName(assemblyName);
#pragma warning restore 618,612

                if (assembly == null)
                {
                    Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly a in loadedAssemblies)
                    {
                        if (a.FullName == assemblyName)
                        {
                            assembly = a;
                            break;
                        }
                    }
                }

                if (assembly == null)
                {
                    throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, assemblyName));
                }

                Type type = assembly.GetType(typeName);
                if (type == null)
                {
                    throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeName, assembly.FullName));
                }

                return type;
            }
            return Type.GetType(typeName);
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return _typeCache.Get(new TypeNameKey(assemblyName, typeName));
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }

        #endregion

        #region Вложенный класс: TypeNameKey

        internal struct TypeNameKey : IEquatable<TypeNameKey>
        {
            #region Поля

            internal readonly string AssemblyName;

            internal readonly string TypeName;

            #endregion

            #region Конструктор

            public TypeNameKey(string assemblyName, string typeName)
            {
                AssemblyName = assemblyName;
                TypeName = typeName;
            }

            #endregion

            #region Методы

            public override int GetHashCode()
            {
                return ((AssemblyName != null) ? AssemblyName.GetHashCode() : 0)
                       ^ ((TypeName != null) ? TypeName.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TypeNameKey))
                {
                    return false;
                }

                return Equals((TypeNameKey)obj);
            }

            public bool Equals(TypeNameKey other)
            {
                return (AssemblyName == other.AssemblyName && TypeName == other.TypeName);
            }

            #endregion
        }

        #endregion
    }
}