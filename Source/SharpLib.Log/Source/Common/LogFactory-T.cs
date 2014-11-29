using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NLog
{
    public class LogFactory<T> : LogFactory where T : Logger
    {
        #region Ìועמהû

        public new T GetLogger(string name)
        {
            return (T)GetLogger(name, typeof(T));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Backwards compatibility")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public new T GetCurrentClassLogger()
        {
            StackFrame frame = new StackFrame(1, false);

            return GetLogger(frame.GetMethod().DeclaringType.FullName);
        }

        #endregion
    }
}