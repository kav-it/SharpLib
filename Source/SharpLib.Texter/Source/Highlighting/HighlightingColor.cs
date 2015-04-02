using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Highlighting
{
    [Serializable]
    public class HighlightingColor : ISerializable, IFreezable, ICloneable, IEquatable<HighlightingColor>
    {
        internal static readonly HighlightingColor Empty = FreezableHelper.FreezeAndReturn(new HighlightingColor());

        private string name;

        private FontWeight? fontWeight;

        private FontStyle? fontStyle;

        private bool? underline;

        private HighlightingBrush foreground;

        private HighlightingBrush background;

        private bool frozen;

        public string Name
        {
            get { return name; }
            set
            {
                if (frozen)
                {
                    throw new InvalidOperationException();
                }
                name = value;
            }
        }

        public FontWeight? FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (frozen)
                {
                    throw new InvalidOperationException();
                }
                fontWeight = value;
            }
        }

        public FontStyle? FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (frozen)
                {
                    throw new InvalidOperationException();
                }
                fontStyle = value;
            }
        }

        public bool? Underline
        {
            get { return underline; }
            set
            {
                if (frozen)
                {
                    throw new InvalidOperationException();
                }
                underline = value;
            }
        }

        public HighlightingBrush Foreground
        {
            get { return foreground; }
            set
            {
                if (frozen)
                {
                    throw new InvalidOperationException();
                }
                foreground = value;
            }
        }

        public HighlightingBrush Background
        {
            get { return background; }
            set
            {
                if (frozen)
                {
                    throw new InvalidOperationException();
                }
                background = value;
            }
        }

        public HighlightingColor()
        {
        }

        protected HighlightingColor(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            Name = info.GetString("Name");
            if (info.GetBoolean("HasWeight"))
            {
                FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(info.GetInt32("Weight"));
            }
            if (info.GetBoolean("HasStyle"))
            {
                FontStyle = (FontStyle?)new FontStyleConverter().ConvertFromInvariantString(info.GetString("Style"));
            }
            if (info.GetBoolean("HasUnderline"))
            {
                Underline = info.GetBoolean("Underline");
            }
            Foreground = (HighlightingBrush)info.GetValue("Foreground", typeof(HighlightingBrush));
            Background = (HighlightingBrush)info.GetValue("Background", typeof(HighlightingBrush));
        }

#if DOTNET4
        [System.Security.SecurityCritical]
#else
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("Name", Name);
            info.AddValue("HasWeight", FontWeight.HasValue);
            if (FontWeight.HasValue)
            {
                info.AddValue("Weight", FontWeight.Value.ToOpenTypeWeight());
            }
            info.AddValue("HasStyle", FontStyle.HasValue);
            if (FontStyle.HasValue)
            {
                info.AddValue("Style", FontStyle.Value.ToString());
            }
            info.AddValue("HasUnderline", Underline.HasValue);
            if (Underline.HasValue)
            {
                info.AddValue("Underline", Underline.Value);
            }
            info.AddValue("Foreground", Foreground);
            info.AddValue("Background", Background);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "CSS usually uses lowercase, and all possible values are English-only")]
        public virtual string ToCss()
        {
            var b = new StringBuilder();
            if (Foreground != null)
            {
                var c = Foreground.GetColor(null);
                if (c != null)
                {
                    b.AppendFormat(CultureInfo.InvariantCulture, "color: #{0:x2}{1:x2}{2:x2}; ", c.Value.R, c.Value.G, c.Value.B);
                }
            }
            if (FontWeight != null)
            {
                b.Append("font-weight: ");
                b.Append(FontWeight.Value.ToString().ToLowerInvariant());
                b.Append("; ");
            }
            if (FontStyle != null)
            {
                b.Append("font-style: ");
                b.Append(FontStyle.Value.ToString().ToLowerInvariant());
                b.Append("; ");
            }
            if (Underline != null)
            {
                b.Append("text-decoration: ");
                b.Append(Underline.Value ? "underline" : "none");
                b.Append("; ");
            }
            return b.ToString();
        }

        public override string ToString()
        {
            return "[" + GetType().Name + " " + (string.IsNullOrEmpty(Name) ? ToCss() : Name) + "]";
        }

        public virtual void Freeze()
        {
            frozen = true;
        }

        public bool IsFrozen
        {
            get { return frozen; }
        }

        public virtual HighlightingColor Clone()
        {
            var c = (HighlightingColor)MemberwiseClone();
            c.frozen = false;
            return c;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override sealed bool Equals(object obj)
        {
            return Equals(obj as HighlightingColor);
        }

        public virtual bool Equals(HighlightingColor other)
        {
            if (other == null)
            {
                return false;
            }
            return name == other.name && fontWeight == other.fontWeight
                   && fontStyle == other.fontStyle && underline == other.underline
                   && object.Equals(foreground, other.foreground) && object.Equals(background, other.background);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked
            {
                if (name != null)
                {
                    hashCode += 1000000007 * name.GetHashCode();
                }
                hashCode += 1000000009 * fontWeight.GetHashCode();
                hashCode += 1000000021 * fontStyle.GetHashCode();
                if (foreground != null)
                {
                    hashCode += 1000000033 * foreground.GetHashCode();
                }
                if (background != null)
                {
                    hashCode += 1000000087 * background.GetHashCode();
                }
            }
            return hashCode;
        }

        public void MergeWith(HighlightingColor color)
        {
            FreezableHelper.ThrowIfFrozen(this);
            if (color.fontWeight != null)
            {
                fontWeight = color.fontWeight;
            }
            if (color.fontStyle != null)
            {
                fontStyle = color.fontStyle;
            }
            if (color.foreground != null)
            {
                foreground = color.foreground;
            }
            if (color.background != null)
            {
                background = color.background;
            }
            if (color.underline != null)
            {
                underline = color.underline;
            }
        }

        internal bool IsEmptyForMerge
        {
            get
            {
                return fontWeight == null && fontStyle == null && underline == null
                       && foreground == null && background == null;
            }
        }
    }
}