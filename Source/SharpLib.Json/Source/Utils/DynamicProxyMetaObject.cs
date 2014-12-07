using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace SharpLib.Json
{
    internal sealed class DynamicProxyMetaObject<T> : DynamicMetaObject
    {
        #region Делегаты

        private delegate DynamicMetaObject Fallback(DynamicMetaObject errorSuggestion);

        #endregion

        #region Поля

        private static readonly Expression[] NoArgs = new Expression[0];

        private readonly bool _dontFallbackFirst;

        private readonly DynamicProxy<T> _proxy;

        #endregion

        #region Свойства

        private new T Value
        {
            get { return (T)base.Value; }
        }

        #endregion

        #region Конструктор

        internal DynamicProxyMetaObject(Expression expression, T value, DynamicProxy<T> proxy, bool dontFallbackFirst)
            : base(expression, BindingRestrictions.Empty, value)
        {
            _proxy = proxy;
            _dontFallbackFirst = dontFallbackFirst;
        }

        #endregion

        #region Методы

        private bool IsOverridden(string method)
        {
            return ReflectionUtils.IsMethodOverridden(_proxy.GetType(), typeof(DynamicProxy<T>), method);
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            return IsOverridden("TryGetMember")
                ? CallMethodWithResult("TryGetMember", binder, NoArgs, e => binder.FallbackGetMember(this, e))
                : base.BindGetMember(binder);
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            return IsOverridden("TrySetMember")
                ? CallMethodReturnLast("TrySetMember", binder, GetArgs(value), e => binder.FallbackSetMember(this, value, e))
                : base.BindSetMember(binder, value);
        }

        public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
        {
            return IsOverridden("TryDeleteMember")
                ? CallMethodNoResult("TryDeleteMember", binder, NoArgs, e => binder.FallbackDeleteMember(this, e))
                : base.BindDeleteMember(binder);
        }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            return IsOverridden("TryConvert")
                ? CallMethodWithResult("TryConvert", binder, NoArgs, e => binder.FallbackConvert(this, e))
                : base.BindConvert(binder);
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (!IsOverridden("TryInvokeMember"))
            {
                return base.BindInvokeMember(binder, args);
            }

            Fallback fallback = e => binder.FallbackInvokeMember(this, args, e);

            DynamicMetaObject call = BuildCallMethodWithResult(
                "TryInvokeMember",
                binder,
                GetArgArray(args),
                BuildCallMethodWithResult(
                    "TryGetMember",
                    new GetBinderAdapter(binder),
                    NoArgs,
                    fallback(null),
                    e => binder.FallbackInvoke(e, args, null)
                    ),
                null
                );

            return _dontFallbackFirst ? call : fallback(call);
        }

        public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
        {
            return IsOverridden("TryCreateInstance")
                ? CallMethodWithResult("TryCreateInstance", binder, GetArgArray(args), e => binder.FallbackCreateInstance(this, args, e))
                : base.BindCreateInstance(binder, args);
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        {
            return IsOverridden("TryInvoke")
                ? CallMethodWithResult("TryInvoke", binder, GetArgArray(args), e => binder.FallbackInvoke(this, args, e))
                : base.BindInvoke(binder, args);
        }

        public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
        {
            return IsOverridden("TryBinaryOperation")
                ? CallMethodWithResult("TryBinaryOperation", binder, GetArgs(arg), e => binder.FallbackBinaryOperation(this, arg, e))
                : base.BindBinaryOperation(binder, arg);
        }

        public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
        {
            return IsOverridden("TryUnaryOperation")
                ? CallMethodWithResult("TryUnaryOperation", binder, NoArgs, e => binder.FallbackUnaryOperation(this, e))
                : base.BindUnaryOperation(binder);
        }

        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            return IsOverridden("TryGetIndex")
                ? CallMethodWithResult("TryGetIndex", binder, GetArgArray(indexes), e => binder.FallbackGetIndex(this, indexes, e))
                : base.BindGetIndex(binder, indexes);
        }

        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            return IsOverridden("TrySetIndex")
                ? CallMethodReturnLast("TrySetIndex", binder, GetArgArray(indexes, value), e => binder.FallbackSetIndex(this, indexes, value, e))
                : base.BindSetIndex(binder, indexes, value);
        }

        public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
        {
            return IsOverridden("TryDeleteIndex")
                ? CallMethodNoResult("TryDeleteIndex", binder, GetArgArray(indexes), e => binder.FallbackDeleteIndex(this, indexes, e))
                : base.BindDeleteIndex(binder, indexes);
        }

        private static UnaryExpression[] GetArgs(params DynamicMetaObject[] args)
        {
            return args.Select(arg => Expression.Convert(arg.Expression, typeof(object))).ToArray();
        }

        private static Expression[] GetArgArray(DynamicMetaObject[] args)
        {
            return new Expression[] { Expression.NewArrayInit(typeof(object), GetArgs(args)) };
        }

        private static Expression[] GetArgArray(DynamicMetaObject[] args, DynamicMetaObject value)
        {
            return new Expression[]
            {
                Expression.NewArrayInit(typeof(object), GetArgs(args)),
                Expression.Convert(value.Expression, typeof(object))
            };
        }

        private static ConstantExpression Constant(DynamicMetaObjectBinder binder)
        {
            Type t = binder.GetType();
            while (!t.IsVisible())
            {
                t = t.BaseType();
            }
            return Expression.Constant(binder, t);
        }

        private DynamicMetaObject CallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, IEnumerable<Expression> args, Fallback fallback, Fallback fallbackInvoke = null)
        {
            DynamicMetaObject fallbackResult = fallback(null);

            DynamicMetaObject callDynamic = BuildCallMethodWithResult(methodName, binder, args, fallbackResult, fallbackInvoke);

            return _dontFallbackFirst ? callDynamic : fallback(callDynamic);
        }

        private DynamicMetaObject BuildCallMethodWithResult(string methodName, DynamicMetaObjectBinder binder, IEnumerable<Expression> args, DynamicMetaObject fallbackResult, Fallback fallbackInvoke)
        {
            ParameterExpression result = Expression.Parameter(typeof(object), null);

            IList<Expression> callArgs = new List<Expression>();
            callArgs.Add(Expression.Convert(Expression, typeof(T)));
            callArgs.Add(Constant(binder));
            callArgs.AddRange(args);
            callArgs.Add(result);

            DynamicMetaObject resultMetaObject = new DynamicMetaObject(result, BindingRestrictions.Empty);

            if (binder.ReturnType != typeof(object))
            {
                UnaryExpression convert = Expression.Convert(resultMetaObject.Expression, binder.ReturnType);

                resultMetaObject = new DynamicMetaObject(convert, resultMetaObject.Restrictions);
            }

            if (fallbackInvoke != null)
            {
                resultMetaObject = fallbackInvoke(resultMetaObject);
            }

            DynamicMetaObject callDynamic = new DynamicMetaObject(
                Expression.Block(
                    new[] { result },
                    Expression.Condition(
                        Expression.Call(
                            Expression.Constant(_proxy),
                            typeof(DynamicProxy<T>).GetMethod(methodName),
                            callArgs
                            ),
                        resultMetaObject.Expression,
                        fallbackResult.Expression,
                        binder.ReturnType
                        )
                    ),
                GetRestrictions().Merge(resultMetaObject.Restrictions).Merge(fallbackResult.Restrictions)
                );

            return callDynamic;
        }

        private DynamicMetaObject CallMethodReturnLast(string methodName, DynamicMetaObjectBinder binder, Expression[] args, Fallback fallback)
        {
            DynamicMetaObject fallbackResult = fallback(null);

            ParameterExpression result = Expression.Parameter(typeof(object), null);

            IList<Expression> callArgs = new List<Expression>();
            callArgs.Add(Expression.Convert(Expression, typeof(T)));
            callArgs.Add(Constant(binder));
            callArgs.AddRange(args);
            callArgs[args.Length + 1] = Expression.Assign(result, callArgs[args.Length + 1]);

            DynamicMetaObject callDynamic = new DynamicMetaObject(
                Expression.Block(
                    new[] { result },
                    Expression.Condition(
                        Expression.Call(
                            Expression.Constant(_proxy),
                            typeof(DynamicProxy<T>).GetMethod(methodName),
                            callArgs
                            ),
                        result,
                        fallbackResult.Expression,
                        typeof(object)
                        )
                    ),
                GetRestrictions().Merge(fallbackResult.Restrictions)
                );

            return _dontFallbackFirst ? callDynamic : fallback(callDynamic);
        }

        private DynamicMetaObject CallMethodNoResult(string methodName, DynamicMetaObjectBinder binder, IEnumerable<Expression> args, Fallback fallback)
        {
            DynamicMetaObject fallbackResult = fallback(null);

            IList<Expression> callArgs = new List<Expression>();
            callArgs.Add(Expression.Convert(Expression, typeof(T)));
            callArgs.Add(Constant(binder));
            callArgs.AddRange(args);

            DynamicMetaObject callDynamic = new DynamicMetaObject(
                Expression.Condition(
                    Expression.Call(
                        Expression.Constant(_proxy),
                        typeof(DynamicProxy<T>).GetMethod(methodName),
                        callArgs
                        ),
                    Expression.Empty(),
                    fallbackResult.Expression,
                    typeof(void)
                    ),
                GetRestrictions().Merge(fallbackResult.Restrictions)
                );

            return _dontFallbackFirst ? callDynamic : fallback(callDynamic);
        }

        private BindingRestrictions GetRestrictions()
        {
            return (Value == null && HasValue)
                ? BindingRestrictions.GetInstanceRestriction(Expression, null)
                : BindingRestrictions.GetTypeRestriction(Expression, LimitType);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _proxy.GetDynamicMemberNames(Value);
        }

        #endregion

        #region Вложенный класс: GetBinderAdapter

        private sealed class GetBinderAdapter : GetMemberBinder
        {
            #region Конструктор

            internal GetBinderAdapter(InvokeMemberBinder binder) :
                base(binder.Name, binder.IgnoreCase)
            {
            }

            #endregion

            #region Методы

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion
    }
}