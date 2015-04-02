using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using SharpLib.Texter.Editing;

namespace SharpLib.Texter.Search
{
    class SearchPanelAdorner : Adorner
    {
        readonly SearchPanel panel;
		
        public SearchPanelAdorner(TextArea textArea, SearchPanel panel)
            : base(textArea)
        {
            this.panel = panel;
            AddVisualChild(panel);
        }
		
        protected override int VisualChildrenCount {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException();
            return panel;
        }
		
        protected override Size ArrangeOverride(Size finalSize)
        {
            panel.Arrange(new Rect(new Point(0, 0), finalSize));
            return new Size(panel.ActualWidth, panel.ActualHeight);
        }
    }
}