using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
    internal sealed class InsightWindowTemplateSelector : DataTemplateSelector
    {
        #region ������

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is string)
            {
                return (DataTemplate)((FrameworkElement)container).FindResource("TextBlockTemplate");
            }

            return null;
        }

        #endregion
    }
}