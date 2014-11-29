using NLog.Config;
using NLog.Layouts;

namespace NLog.Filters
{
    public abstract class LayoutBasedFilter : Filter
    {
        #region Свойства

        [RequiredParameter]
        public Layout Layout { get; set; }

        #endregion
    }
}