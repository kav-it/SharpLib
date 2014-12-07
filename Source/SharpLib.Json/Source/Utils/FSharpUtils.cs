using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SharpLib.Json
{
    internal static class FSharpUtils
    {
        #region Константы

        public const string FSharpListTypeName = "FSharpList`1";

        public const string FSharpMapTypeName = "FSharpMap`2";

        public const string FSharpSetTypeName = "FSharpSet`1";

        #endregion

        #region Поля

        private static readonly object Lock = new object();

        private static bool _initialized;

        private static Type _mapType;

        private static MethodInfo _ofSeq;

        #endregion

        #region Свойства

        public static Assembly FSharpCoreAssembly { get; private set; }

        public static MethodCall<object, object> IsUnion { get; private set; }

        public static MethodCall<object, object> GetUnionFields { get; private set; }

        public static MethodCall<object, object> GetUnionCases { get; private set; }

        public static MethodCall<object, object> MakeUnion { get; private set; }

        public static Func<object, object> GetUnionCaseInfoName { get; private set; }

        public static Func<object, object> GetUnionCaseInfo { get; private set; }

        public static Func<object, object> GetUnionCaseFields { get; private set; }

        public static MethodCall<object, object> GetUnionCaseInfoFields { get; private set; }

        #endregion

        #region Методы

        public static void EnsureInitialized(Assembly fsharpCoreAssembly)
        {
            if (!_initialized)
            {
                lock (Lock)
                {
                    if (!_initialized)
                    {
                        FSharpCoreAssembly = fsharpCoreAssembly;

                        Type fsharpType = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpType");

                        MethodInfo isUnionMethodInfo = fsharpType.GetMethod("IsUnion", BindingFlags.Public | BindingFlags.Static);
                        IsUnion = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(isUnionMethodInfo);

                        MethodInfo getUnionCasesMethodInfo = fsharpType.GetMethod("GetUnionCases", BindingFlags.Public | BindingFlags.Static);
                        GetUnionCases = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(getUnionCasesMethodInfo);

                        Type fsharpValue = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.FSharpValue");

                        MethodInfo getUnionFieldsMethodInfo = fsharpValue.GetMethod("GetUnionFields", BindingFlags.Public | BindingFlags.Static);
                        GetUnionFields = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(getUnionFieldsMethodInfo);

                        GetUnionCaseInfo = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(getUnionFieldsMethodInfo.ReturnType.GetProperty("Item1"));
                        GetUnionCaseFields = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(getUnionFieldsMethodInfo.ReturnType.GetProperty("Item2"));

                        MethodInfo makeUnionMethodInfo = fsharpValue.GetMethod("MakeUnion", BindingFlags.Public | BindingFlags.Static);
                        MakeUnion = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(makeUnionMethodInfo);

                        Type unionCaseInfo = fsharpCoreAssembly.GetType("Microsoft.FSharp.Reflection.UnionCaseInfo");

                        GetUnionCaseInfoName = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(unionCaseInfo.GetProperty("Name"));
                        GetUnionCaseInfoFields = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(unionCaseInfo.GetMethod("GetFields"));

                        Type listModule = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.ListModule");
                        _ofSeq = listModule.GetMethod("OfSeq");

                        _mapType = fsharpCoreAssembly.GetType("Microsoft.FSharp.Collections.FSharpMap`2");

                        Thread.MemoryBarrier();
                        _initialized = true;
                    }
                }
            }
        }

        public static ObjectConstructor<object> CreateSeq(Type t)
        {
            MethodInfo seqType = _ofSeq.MakeGenericMethod(t);

            return JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(seqType);
        }

        public static ObjectConstructor<object> CreateMap(Type keyType, Type valueType)
        {
            MethodInfo creatorDefinition = typeof(FSharpUtils).GetMethod("BuildMapCreator");

            MethodInfo creatorGeneric = creatorDefinition.MakeGenericMethod(keyType, valueType);

            return (ObjectConstructor<object>)creatorGeneric.Invoke(null, null);
        }

        public static ObjectConstructor<object> BuildMapCreator<TKey, TValue>()
        {
            Type genericMapType = _mapType.MakeGenericType(typeof(TKey), typeof(TValue));
            ConstructorInfo ctor = genericMapType.GetConstructor(new[] { typeof(IEnumerable<Tuple<TKey, TValue>>) });
            ObjectConstructor<object> ctorDelegate = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(ctor);

            ObjectConstructor<object> creator = args =>
            {
                IEnumerable<KeyValuePair<TKey, TValue>> values = (IEnumerable<KeyValuePair<TKey, TValue>>)args[0];
                IEnumerable<Tuple<TKey, TValue>> tupleValues = values.Select(kv => new Tuple<TKey, TValue>(kv.Key, kv.Value));

                return ctorDelegate(tupleValues);
            };

            return creator;
        }

        #endregion
    }
}