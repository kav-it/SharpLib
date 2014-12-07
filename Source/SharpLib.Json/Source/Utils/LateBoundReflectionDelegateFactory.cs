using System;
using System.Reflection;

namespace SharpLib.Json
{
    internal class LateBoundReflectionDelegateFactory : ReflectionDelegateFactory
    {
        #region Поля

        private static readonly LateBoundReflectionDelegateFactory _instance = new LateBoundReflectionDelegateFactory();

        #endregion

        #region Свойства

        internal static ReflectionDelegateFactory Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Методы

        public override ObjectConstructor<object> CreateParametrizedConstructor(MethodBase method)
        {
            ValidationUtils.ArgumentNotNull(method, "method");

            ConstructorInfo c = method as ConstructorInfo;
            if (c != null)
            {
                return c.Invoke;
            }

            return a => method.Invoke(null, a);
        }

        public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
        {
            ValidationUtils.ArgumentNotNull(method, "method");

            ConstructorInfo c = method as ConstructorInfo;
            if (c != null)
            {
                return (o, a) => c.Invoke(a);
            }

            return (o, a) => method.Invoke(o, a);
        }

        public override Func<T> CreateDefaultConstructor<T>(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            if (type.IsValueType())
            {
                return () => (T)Activator.CreateInstance(type);
            }

            ConstructorInfo constructorInfo = ReflectionUtils.GetDefaultConstructor(type, true);

            return () => (T)constructorInfo.Invoke(null);
        }

        public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");

            return o => propertyInfo.GetValue(o, null);
        }

        public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
        {
            ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");

            return o => fieldInfo.GetValue(o);
        }

        public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
        {
            ValidationUtils.ArgumentNotNull(fieldInfo, "fieldInfo");

            return (o, v) => fieldInfo.SetValue(o, v);
        }

        public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
        {
            ValidationUtils.ArgumentNotNull(propertyInfo, "propertyInfo");

            return (o, v) => propertyInfo.SetValue(o, v, null);
        }

        #endregion
    }
}