using System.Text;

namespace SharpLib.Log
{
    public abstract class WrapperLayoutRendererBase : LayoutRenderer
    {
        #region Ñגמיסעגא

        [DefaultParameter]
        public Layout Inner { get; set; }

        #endregion

        #region Ìועמהû

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            string msg = RenderInner(logEvent);
            builder.Append(Transform(msg));
        }

        protected abstract string Transform(string text);

        protected virtual string RenderInner(LogEventInfo logEvent)
        {
            return Inner.Render(logEvent);
        }

        #endregion
    }
}