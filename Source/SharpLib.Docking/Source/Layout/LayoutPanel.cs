using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SharpLib.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutPanel : LayoutPositionableGroup<ILayoutPanelElement>, ILayoutPanelElement, ILayoutOrientableGroup
    {
        #region Поля

        private Orientation _orientation;

        #endregion

        #region Свойства

        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation != value)
                {
                    RaisePropertyChanging("Orientation");
                    _orientation = value;
                    RaisePropertyChanged("Orientation");
                }
            }
        }

        #endregion

        #region Конструктор

        public LayoutPanel()
        {
        }

        public LayoutPanel(ILayoutPanelElement firstChild)
        {
            Children.Add(firstChild);
        }

        #endregion

        #region Методы

        protected override bool GetVisibility()
        {
            return Children.Any(c => c.IsVisible);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Orientation", Orientation.ToString());
            base.WriteXml(writer);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Orientation"))
            {
                Orientation = (Orientation)Enum.Parse(typeof(Orientation), reader.Value, true);
            }
            base.ReadXml(reader);
        }

        public override void ConsoleDump(int tab)
        {
            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine(string.Format("Panel({0})", Orientation));

            foreach (var layoutPanelElement in Children)
            {
                var child = (LayoutElement)layoutPanelElement;
                child.ConsoleDump(tab + 1);
            }
        }

        #endregion
    }
}