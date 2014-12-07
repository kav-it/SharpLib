using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SharpLib.Json
{
    internal static class DynamicUtils
    {
        #region Методы

        public static IEnumerable<string> GetDynamicMemberNames(this IDynamicMetaObjectProvider dynamicProvider)
        {
            DynamicMetaObject metaObject = dynamicProvider.GetMetaObject(Expression.Constant(dynamicProvider));
            return metaObject.GetDynamicMemberNames();
        }

        #endregion

        #region Вложенный класс: BinderWrapper

        internal static class BinderWrapper
        {
            public const string CSharpAssemblyName = "Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

            private const string BinderTypeName = "Microsoft.CSharp.RuntimeBinder.Binder, " + CSharpAssemblyName;

            private const string CSharpArgumentInfoTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo, " + CSharpAssemblyName;

            private const string CSharpArgumentInfoFlagsTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, " + CSharpAssemblyName;

            private const string CSharpBinderFlagsTypeName = "Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, " + CSharpAssemblyName;

            private static object _getCSharpArgumentInfoArray;

            private static object _setCSharpArgumentInfoArray;

            private static MethodCall<object, object> _getMemberCall;

            private static MethodCall<object, object> _setMemberCall;

            private static bool _init;

            private static void Init()
            {
                if (!_init)
                {
                    Type binderType = Type.GetType(BinderTypeName, false);
                    if (binderType == null)
                    {
                        throw new InvalidOperationException(
                            "Could not resolve type '{0}'. You may need to add a reference to Microsoft.CSharp.dll to work with dynamic types.".FormatWith(CultureInfo.InvariantCulture, BinderTypeName));
                    }

                    _getCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(0);

                    _setCSharpArgumentInfoArray = CreateSharpArgumentInfoArray(0, 3);
                    CreateMemberCalls();

                    _init = true;
                }
            }

            private static object CreateSharpArgumentInfoArray(params int[] values)
            {
                Type csharpArgumentInfoType = Type.GetType(CSharpArgumentInfoTypeName);
                Type csharpArgumentInfoFlags = Type.GetType(CSharpArgumentInfoFlagsTypeName);

                Array a = Array.CreateInstance(csharpArgumentInfoType, values.Length);

                for (int i = 0; i < values.Length; i++)
                {
                    MethodInfo createArgumentInfoMethod = csharpArgumentInfoType.GetMethod("Create", new[] { csharpArgumentInfoFlags, typeof(string) });
                    object arg = createArgumentInfoMethod.Invoke(null, new object[] { 0, null });
                    a.SetValue(arg, i);
                }

                return a;
            }

            private static void CreateMemberCalls()
            {
                Type csharpArgumentInfoType = Type.GetType(CSharpArgumentInfoTypeName, true);
                Type csharpBinderFlagsType = Type.GetType(CSharpBinderFlagsTypeName, true);
                Type binderType = Type.GetType(BinderTypeName, true);

                Type csharpArgumentInfoTypeEnumerableType = typeof(IEnumerable<>).MakeGenericType(csharpArgumentInfoType);

                MethodInfo getMemberMethod = binderType.GetMethod("GetMember", new[] { csharpBinderFlagsType, typeof(string), typeof(Type), csharpArgumentInfoTypeEnumerableType });
                _getMemberCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(getMemberMethod);

                MethodInfo setMemberMethod = binderType.GetMethod("SetMember", new[] { csharpBinderFlagsType, typeof(string), typeof(Type), csharpArgumentInfoTypeEnumerableType });
                _setMemberCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(setMemberMethod);
            }

            public static CallSiteBinder GetMember(string name, Type context)
            {
                Init();
                return (CallSiteBinder)_getMemberCall(null, 0, name, context, _getCSharpArgumentInfoArray);
            }

            public static CallSiteBinder SetMember(string name, Type context)
            {
                Init();
                return (CallSiteBinder)_setMemberCall(null, 0, name, context, _setCSharpArgumentInfoArray);
            }
        }

        #endregion
    }

    internal class NoThrowGetBinderMember : GetMemberBinder
    {
        #region Поля

        private readonly GetMemberBinder _innerBinder;

        #endregion

        #region Конструктор

        public NoThrowGetBinderMember(GetMemberBinder innerBinder)
            : base(innerBinder.Name, innerBinder.IgnoreCase)
        {
            _innerBinder = innerBinder;
        }

        #endregion

        #region Методы

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            DynamicMetaObject retMetaObject = _innerBinder.Bind(target, new DynamicMetaObject[] { });

            NoThrowExpressionVisitor noThrowVisitor = new NoThrowExpressionVisitor();
            Expression resultExpression = noThrowVisitor.Visit(retMetaObject.Expression);

            DynamicMetaObject finalMetaObject = new DynamicMetaObject(resultExpression, retMetaObject.Restrictions);
            return finalMetaObject;
        }

        #endregion
    }

    internal class NoThrowSetBinderMember : SetMemberBinder
    {
        #region Поля

        private readonly SetMemberBinder _innerBinder;

        #endregion

        #region Конструктор

        public NoThrowSetBinderMember(SetMemberBinder innerBinder)
            : base(innerBinder.Name, innerBinder.IgnoreCase)
        {
            _innerBinder = innerBinder;
        }

        #endregion

        #region Методы

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
            DynamicMetaObject retMetaObject = _innerBinder.Bind(target, new[] { value });

            NoThrowExpressionVisitor noThrowVisitor = new NoThrowExpressionVisitor();
            Expression resultExpression = noThrowVisitor.Visit(retMetaObject.Expression);

            DynamicMetaObject finalMetaObject = new DynamicMetaObject(resultExpression, retMetaObject.Restrictions);
            return finalMetaObject;
        }

        #endregion
    }

    internal class NoThrowExpressionVisitor : ExpressionVisitor
    {
        #region Поля

        internal static readonly object ErrorResult = new object();

        #endregion

        #region Методы

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            if (node.IfFalse.NodeType == ExpressionType.Throw)
            {
                return Expression.Condition(node.Test, node.IfTrue, Expression.Constant(ErrorResult));
            }

            return base.VisitConditional(node);
        }

        #endregion
    }
}