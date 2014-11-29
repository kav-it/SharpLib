using System;

#pragma warning disable 1591

namespace JetBrains.Annotations
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter |
        AttributeTargets.Property | AttributeTargets.Delegate |
        AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    internal sealed class CanBeNullAttribute : Attribute
    {
    }

    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Parameter |
        AttributeTargets.Property | AttributeTargets.Delegate |
        AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    internal sealed class NotNullAttribute : Attribute
    {
    }

    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Method,
        AllowMultiple = false, Inherited = true)]
    internal sealed class StringFormatMethodAttribute : Attribute
    {
        #region Свойства

        public string FormatParameterName { get; private set; }

        #endregion

        #region Конструктор

        public StringFormatMethodAttribute(string formatParameterName)
        {
            FormatParameterName = formatParameterName;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    internal sealed class InvokerParameterNameAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
    {
        #region Свойства

        public string ParameterName { get; private set; }

        #endregion

        #region Конструктор

        public NotifyPropertyChangedInvocatorAttribute()
        {
        }

        public NotifyPropertyChangedInvocatorAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    internal sealed class ContractAnnotationAttribute : Attribute
    {
        #region Свойства

        public string Contract { get; private set; }

        public bool ForceFullStates { get; private set; }

        #endregion

        #region Конструктор

        public ContractAnnotationAttribute([NotNull] string contract)
            : this(contract, false)
        {
        }

        public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates)
        {
            Contract = contract;
            ForceFullStates = forceFullStates;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    internal sealed class LocalizationRequiredAttribute : Attribute
    {
        #region Свойства

        public bool Required { get; private set; }

        #endregion

        #region Конструктор

        public LocalizationRequiredAttribute()
            : this(true)
        {
        }

        public LocalizationRequiredAttribute(bool required)
        {
            Required = required;
        }

        #endregion
    }

    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class |
        AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    internal sealed class CannotApplyEqualityOperatorAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [BaseTypeRequired(typeof(Attribute))]
    internal sealed class BaseTypeRequiredAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public Type BaseType { get; private set; }

        #endregion

        #region Конструктор

        public BaseTypeRequiredAttribute([NotNull] Type baseType)
        {
            BaseType = baseType;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    internal sealed class UsedImplicitlyAttribute : Attribute
    {
        #region Свойства

        public ImplicitUseKindFlags UseKindFlags { get; private set; }

        public ImplicitUseTargetFlags TargetFlags { get; private set; }

        #endregion

        #region Конструктор

        public UsedImplicitlyAttribute()
            : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
        {
        }

        public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
            : this(useKindFlags, ImplicitUseTargetFlags.Default)
        {
        }

        public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
            : this(ImplicitUseKindFlags.Default, targetFlags)
        {
        }

        public UsedImplicitlyAttribute(
            ImplicitUseKindFlags useKindFlags,
            ImplicitUseTargetFlags targetFlags)
        {
            UseKindFlags = useKindFlags;
            TargetFlags = targetFlags;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal sealed class MeansImplicitUseAttribute : Attribute
    {
        #region Свойства

        [UsedImplicitly]
        public ImplicitUseKindFlags UseKindFlags { get; private set; }

        [UsedImplicitly]
        public ImplicitUseTargetFlags TargetFlags { get; private set; }

        #endregion

        #region Конструктор

        public MeansImplicitUseAttribute()
            : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
        {
        }

        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
            : this(useKindFlags, ImplicitUseTargetFlags.Default)
        {
        }

        public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
            : this(ImplicitUseKindFlags.Default, targetFlags)
        {
        }

        public MeansImplicitUseAttribute(
            ImplicitUseKindFlags useKindFlags,
            ImplicitUseTargetFlags targetFlags)
        {
            UseKindFlags = useKindFlags;
            TargetFlags = targetFlags;
        }

        #endregion
    }

    [Flags]
    internal enum ImplicitUseKindFlags
    {
        Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

        Access = 1,

        Assign = 2,

        InstantiatedWithFixedConstructorSignature = 4,

        InstantiatedNoFixedConstructorSignature = 8,
    }

    [Flags]
    internal enum ImplicitUseTargetFlags
    {
        Default = Itself,

        Itself = 1,

        Members = 2,

        WithMembers = Itself | Members
    }

    [MeansImplicitUse]
    internal sealed class PublicAPIAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string Comment { get; private set; }

        #endregion

        #region Конструктор

        public PublicAPIAttribute()
        {
        }

        public PublicAPIAttribute([NotNull] string comment)
        {
            Comment = comment;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
    internal sealed class InstantHandleAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    internal sealed class PureAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal class PathReferenceAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string BasePath { get; private set; }

        #endregion

        #region Конструктор

        public PathReferenceAttribute()
        {
        }

        public PathReferenceAttribute([PathReference] string basePath)
        {
            BasePath = basePath;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcActionAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string AnonymousProperty { get; private set; }

        #endregion

        #region Конструктор

        public AspMvcActionAttribute()
        {
        }

        public AspMvcActionAttribute([NotNull] string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AspMvcAreaAttribute : PathReferenceAttribute
    {
        #region Свойства

        [NotNull]
        public string AnonymousProperty { get; private set; }

        #endregion

        #region Конструктор

        public AspMvcAreaAttribute()
        {
        }

        public AspMvcAreaAttribute([NotNull] string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcControllerAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string AnonymousProperty { get; private set; }

        #endregion

        #region Конструктор

        public AspMvcControllerAttribute()
        {
        }

        public AspMvcControllerAttribute([NotNull] string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AspMvcMasterAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AspMvcModelTypeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcPartialViewAttribute : PathReferenceAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class AspMvcSupressViewErrorAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AspMvcDisplayTemplateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AspMvcEditorTemplateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class AspMvcTemplateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    internal sealed class AspMvcViewAttribute : PathReferenceAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    internal sealed class AspMvcActionSelectorAttribute : Attribute
    {
    }

    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property |
        AttributeTargets.Field, Inherited = true)]
    internal sealed class HtmlElementAttributesAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string Name { get; private set; }

        #endregion

        #region Конструктор

        public HtmlElementAttributesAttribute()
        {
        }

        public HtmlElementAttributesAttribute([NotNull] string name)
        {
            Name = name;
        }

        #endregion
    }

    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field |
        AttributeTargets.Property, Inherited = true)]
    internal sealed class HtmlAttributeValueAttribute : Attribute
    {
        #region Свойства

        [NotNull]
        public string Name { get; private set; }

        #endregion

        #region Конструктор

        public HtmlAttributeValueAttribute([NotNull] string name)
        {
            Name = name;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, Inherited = true)]
    internal sealed class RazorSectionAttribute : Attribute
    {
    }
}