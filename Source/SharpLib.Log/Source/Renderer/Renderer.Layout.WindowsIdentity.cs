using System.ComponentModel;
using System.Security.Principal;
using System.Text;

namespace SharpLib.Log
{
    [LayoutRenderer("windows-identity")]
    public class WindowsIdentityLayoutRenderer : LayoutRenderer
    {
        #region Свойства

        [DefaultValue(true)]
        public bool Domain { get; set; }

        [DefaultValue(true)]
        public bool UserName { get; set; }

        #endregion

        #region Конструктор

        public WindowsIdentityLayoutRenderer()
        {
            UserName = true;
            Domain = true;
        }

        #endregion

        #region Методы

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity != null)
            {
                string output;

                if (UserName)
                {
                    if (Domain)
                    {
                        output = currentIdentity.Name;
                    }
                    else
                    {
                        int pos = currentIdentity.Name.LastIndexOf('\\');
                        output = pos >= 0 ? currentIdentity.Name.Substring(pos + 1) : currentIdentity.Name;
                    }
                }
                else
                {
                    if (!Domain)
                    {
                        return;
                    }

                    int pos = currentIdentity.Name.IndexOf('\\');
                    output = pos >= 0 ? currentIdentity.Name.Substring(0, pos) : currentIdentity.Name;
                }

                builder.Append(output);
            }
        }

        #endregion
    }
}