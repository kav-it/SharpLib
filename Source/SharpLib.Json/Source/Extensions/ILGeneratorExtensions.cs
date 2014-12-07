using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SharpLib.Json
{
    internal static class ILGeneratorExtensions
    {
        #region Методы

        public static void PushInstance(this ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(type.IsValueType() ? OpCodes.Unbox : OpCodes.Castclass, type);
        }

        public static void PushArrayInstance(this ILGenerator generator, int argsIndex, int arrayIndex)
        {
            generator.Emit(OpCodes.Ldarg, argsIndex);
            generator.Emit(OpCodes.Ldc_I4, arrayIndex);
            generator.Emit(OpCodes.Ldelem_Ref);
        }

        public static void BoxIfNeeded(this ILGenerator generator, Type type)
        {
            generator.Emit(type.IsValueType() ? OpCodes.Box : OpCodes.Castclass, type);
        }

        public static void UnboxIfNeeded(this ILGenerator generator, Type type)
        {
            generator.Emit(type.IsValueType() ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        public static void CallMethod(this ILGenerator generator, MethodInfo methodInfo)
        {
            if (methodInfo.IsFinal || !methodInfo.IsVirtual)
            {
                generator.Emit(OpCodes.Call, methodInfo);
            }
            else
            {
                generator.Emit(OpCodes.Callvirt, methodInfo);
            }
        }

        public static void Return(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ret);
        }

        #endregion
    }
}