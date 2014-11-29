using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal sealed class MeansImplicitUseAttribute : Attribute
    {
        #region ��������

        [UsedImplicitly]
        public ImplicitUseKindFlags UseKindFlags { get; private set; }

        [UsedImplicitly]
        public ImplicitUseTargetFlags TargetFlags { get; private set; }

        #endregion

        #region �����������

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
}