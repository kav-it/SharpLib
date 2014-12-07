using System;
using System.Globalization;
using System.Reflection;

namespace SharpLib.Json
{
    public class ReflectionValueProvider : IValueProvider
    {
        #region Поля

        private readonly MemberInfo _memberInfo;

        #endregion

        #region Конструктор

        public ReflectionValueProvider(MemberInfo memberInfo)
        {
            ValidationUtils.ArgumentNotNull(memberInfo, "memberInfo");
            _memberInfo = memberInfo;
        }

        #endregion

        #region Методы

        public void SetValue(object target, object value)
        {
            try
            {
                ReflectionUtils.SetMemberValue(_memberInfo, target, value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Error setting value to '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, _memberInfo.Name, target.GetType()), ex);
            }
        }

        public object GetValue(object target)
        {
            try
            {
                return ReflectionUtils.GetMemberValue(_memberInfo, target);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Error getting value from '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture, _memberInfo.Name, target.GetType()), ex);
            }
        }

        #endregion
    }
}