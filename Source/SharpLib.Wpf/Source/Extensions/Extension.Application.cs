using System.Linq;
using System.Windows;

namespace SharpLibWpf
{
    public static class ApplicationExtension
    {
        #region Методы

        public static Window GetActiveWindowEx(this Application value)
        {
            return Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        }

        #endregion
    }
}