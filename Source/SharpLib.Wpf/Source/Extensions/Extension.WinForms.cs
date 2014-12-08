
namespace SharpLib.Wpf
{
    public static class ExtensionWinForms
    {
        public static System.Drawing.Point ToSystemDrawing(this System.Windows.Point p)
        {
            return new System.Drawing.Point((int)p.X, (int)p.Y);
        }

        public static System.Drawing.Size ToSystemDrawing(this System.Windows.Size s)
        {
            return new System.Drawing.Size((int)s.Width, (int)s.Height);
        }

        public static System.Drawing.Rectangle ToSystemDrawing(this System.Windows.Rect r)
        {
            return new System.Drawing.Rectangle(r.TopLeft.ToSystemDrawing(), r.Size.ToSystemDrawing());
        }

        public static System.Drawing.Color ToSystemDrawing(this System.Windows.Media.Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static System.Windows.Point ToWpf(this System.Drawing.Point p)
        {
            return new System.Windows.Point(p.X, p.Y);
        }

        public static System.Windows.Size ToWpf(this System.Drawing.Size s)
        {
            return new System.Windows.Size(s.Width, s.Height);
        }

        public static System.Windows.Rect ToWpf(this System.Drawing.Rectangle rect)
        {
            return new System.Windows.Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
        }

        public static System.Windows.Media.Color ToWpf(this System.Drawing.Color c)
        {
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        
    }
}