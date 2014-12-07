using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SharpLib.Json
{
    internal class ReflectionMember
    {
        #region Свойства

        public Type MemberType { get; set; }

        public Func<object, object> Getter { get; set; }

        public Action<object, object> Setter { get; set; }

        #endregion
    }

    internal class ReflectionObject
    {
        #region Свойства

        public ObjectConstructor<object> Creator { get; private set; }

        public IDictionary<string, ReflectionMember> Members { get; private set; }

        #endregion

        #region Конструктор

        public ReflectionObject()
        {
            Members = new Dictionary<string, ReflectionMember>();
        }

        #endregion

        #region Методы

        public object GetValue(object target, string member)
        {
            Func<object, object> getter = Members[member].Getter;
            return getter(target);
        }

        public void SetValue(object target, string member, object value)
        {
            Action<object, object> setter = Members[member].Setter;
            setter(target, value);
        }

        public Type GetType(string member)
        {
            return Members[member].MemberType;
        }

        public static ReflectionObject Create(Type t, params string[] memberNames)
        {
            return Create(t, null, memberNames);
        }

        public static ReflectionObject Create(Type t, MethodBase creator, params string[] memberNames)
        {
            ReflectionObject d = new ReflectionObject();

            ReflectionDelegateFactory delegateFactory = JsonTypeReflector.ReflectionDelegateFactory;

            if (creator != null)
            {
                d.Creator = delegateFactory.CreateParametrizedConstructor(creator);
            }
            else
            {
                if (ReflectionUtils.HasDefaultConstructor(t, false))
                {
                    Func<object> ctor = delegateFactory.CreateDefaultConstructor<object>(t);

                    d.Creator = args => ctor();
                }
            }

            foreach (string memberName in memberNames)
            {
                MemberInfo[] members = t.GetMember(memberName, BindingFlags.Instance | BindingFlags.Public);
                if (members.Length != 1)
                {
                    throw new ArgumentException("Expected a single member with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, memberName));
                }

                MemberInfo member = members.Single();

                ReflectionMember reflectionMember = new ReflectionMember();

                switch (member.MemberType())
                {
                    case MemberTypes.Field:
                    case MemberTypes.Property:
                        if (ReflectionUtils.CanReadMemberValue(member, false))
                        {
                            reflectionMember.Getter = delegateFactory.CreateGet<object>(member);
                        }

                        if (ReflectionUtils.CanSetMemberValue(member, false, false))
                        {
                            reflectionMember.Setter = delegateFactory.CreateSet<object>(member);
                        }
                        break;
                    case MemberTypes.Method:
                        MethodInfo method = (MethodInfo)member;
                        if (method.IsPublic)
                        {
                            ParameterInfo[] parameters = method.GetParameters();
                            if (parameters.Length == 0 && method.ReturnType != typeof(void))
                            {
                                MethodCall<object, object> call = delegateFactory.CreateMethodCall<object>(method);
                                reflectionMember.Getter = target => call(target);
                            }
                            else if (parameters.Length == 1 && method.ReturnType == typeof(void))
                            {
                                MethodCall<object, object> call = delegateFactory.CreateMethodCall<object>(method);
                                reflectionMember.Setter = (target, arg) => call(target, arg);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentException("Unexpected member type '{0}' for member '{1}'.".FormatWith(CultureInfo.InvariantCulture, member.MemberType(), member.Name));
                }

                if (ReflectionUtils.CanReadMemberValue(member, false))
                {
                    reflectionMember.Getter = delegateFactory.CreateGet<object>(member);
                }

                if (ReflectionUtils.CanSetMemberValue(member, false, false))
                {
                    reflectionMember.Setter = delegateFactory.CreateSet<object>(member);
                }

                reflectionMember.MemberType = ReflectionUtils.GetMemberUnderlyingType(member);

                d.Members[memberName] = reflectionMember;
            }

            return d;
        }

        #endregion
    }
}