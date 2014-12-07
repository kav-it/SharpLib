using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace SharpLib.Json
{
    public class JsonDynamicContract : JsonContainerContract
    {
        #region Поля

        private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>> _callSiteGetters =
            new ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>>(CreateCallSiteGetter);

        private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>> _callSiteSetters =
            new ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>>(CreateCallSiteSetter);

        #endregion

        #region Свойства

        public JsonPropertyCollection Properties { get; private set; }

        public Func<string, string> PropertyNameResolver { get; set; }

        #endregion

        #region Конструктор

        public JsonDynamicContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Dynamic;

            Properties = new JsonPropertyCollection(UnderlyingType);
        }

        #endregion

        #region Методы

        private static CallSite<Func<CallSite, object, object>> CreateCallSiteGetter(string name)
        {
            GetMemberBinder getMemberBinder = (GetMemberBinder)DynamicUtils.BinderWrapper.GetMember(name, typeof(DynamicUtils));

            return CallSite<Func<CallSite, object, object>>.Create(new NoThrowGetBinderMember(getMemberBinder));
        }

        private static CallSite<Func<CallSite, object, object, object>> CreateCallSiteSetter(string name)
        {
            SetMemberBinder binder = (SetMemberBinder)DynamicUtils.BinderWrapper.SetMember(name, typeof(DynamicUtils));

            return CallSite<Func<CallSite, object, object, object>>.Create(new NoThrowSetBinderMember(binder));
        }

        internal bool TryGetMember(IDynamicMetaObjectProvider dynamicProvider, string name, out object value)
        {
            ValidationUtils.ArgumentNotNull(dynamicProvider, "dynamicProvider");

            CallSite<Func<CallSite, object, object>> callSite = _callSiteGetters.Get(name);

            object result = callSite.Target(callSite, dynamicProvider);

            if (!ReferenceEquals(result, NoThrowExpressionVisitor.ErrorResult))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }

        internal bool TrySetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value)
        {
            ValidationUtils.ArgumentNotNull(dynamicProvider, "dynamicProvider");

            CallSite<Func<CallSite, object, object, object>> callSite = _callSiteSetters.Get(name);

            object result = callSite.Target(callSite, dynamicProvider, value);

            return !ReferenceEquals(result, NoThrowExpressionVisitor.ErrorResult);
        }

        #endregion
    }
}