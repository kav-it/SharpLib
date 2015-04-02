using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.CodeCompletion
{
    public class CompletionList : Control
    {
        #region Поля

        public static readonly DependencyProperty EmptyTemplateProperty =
            DependencyProperty.Register("EmptyTemplate", typeof(ControlTemplate), typeof(CompletionList),
                new FrameworkPropertyMetadata());

        private readonly ObservableCollection<ICompletionData> completionData = new ObservableCollection<ICompletionData>();

        private ObservableCollection<ICompletionData> currentList;

        private string currentText;

        private bool isFiltering = true;

        private CompletionListBox listBox;

        #endregion

        #region Свойства

        public bool IsFiltering
        {
            get { return isFiltering; }
            set { isFiltering = value; }
        }

        public ControlTemplate EmptyTemplate
        {
            get { return (ControlTemplate)GetValue(EmptyTemplateProperty); }
            set { SetValue(EmptyTemplateProperty, value); }
        }

        public CompletionListBox ListBox
        {
            get
            {
                if (listBox == null)
                {
                    ApplyTemplate();
                }
                return listBox;
            }
        }

        public ScrollViewer ScrollViewer
        {
            get { return listBox != null ? listBox.scrollViewer : null; }
        }

        public IList<ICompletionData> CompletionData
        {
            get { return completionData; }
        }

        public ICompletionData SelectedItem
        {
            get { return (listBox != null ? listBox.SelectedItem : null) as ICompletionData; }
            set
            {
                if (listBox == null && value != null)
                {
                    ApplyTemplate();
                }
                if (listBox != null)
                {
                    listBox.SelectedItem = value;
                }
            }
        }

        #endregion

        #region События

        public event EventHandler InsertionRequested;

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(Selector.SelectionChangedEvent, value); }
            remove { RemoveHandler(Selector.SelectionChangedEvent, value); }
        }

        #endregion

        #region Конструктор

        static CompletionList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompletionList),
                new FrameworkPropertyMetadata(typeof(CompletionList)));
        }

        #endregion

        #region Методы

        public void RequestInsertion(EventArgs e)
        {
            if (InsertionRequested != null)
            {
                InsertionRequested(this, e);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            listBox = GetTemplateChild("PART_ListBox") as CompletionListBox;
            if (listBox != null)
            {
                listBox.ItemsSource = completionData;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                HandleKey(e);
            }
        }

        public void HandleKey(KeyEventArgs e)
        {
            if (listBox == null)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Down:
                    e.Handled = true;
                    listBox.SelectIndex(listBox.SelectedIndex + 1);
                    break;
                case Key.Up:
                    e.Handled = true;
                    listBox.SelectIndex(listBox.SelectedIndex - 1);
                    break;
                case Key.PageDown:
                    e.Handled = true;
                    listBox.SelectIndex(listBox.SelectedIndex + listBox.VisibleItemCount);
                    break;
                case Key.PageUp:
                    e.Handled = true;
                    listBox.SelectIndex(listBox.SelectedIndex - listBox.VisibleItemCount);
                    break;
                case Key.Home:
                    e.Handled = true;
                    listBox.SelectIndex(0);
                    break;
                case Key.End:
                    e.Handled = true;
                    listBox.SelectIndex(listBox.Items.Count - 1);
                    break;
                case Key.Tab:
                case Key.Enter:
                    e.Handled = true;
                    RequestInsertion(e);
                    break;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.ChangedButton == MouseButton.Left)
            {
                if ((e.OriginalSource as DependencyObject).VisualAncestorsAndSelf().TakeWhile(obj => obj != this).Any(obj => obj is ListBoxItem))
                {
                    e.Handled = true;
                    RequestInsertion(e);
                }
            }
        }

        public void ScrollIntoView(ICompletionData item)
        {
            if (listBox == null)
            {
                ApplyTemplate();
            }
            if (listBox != null)
            {
                listBox.ScrollIntoView(item);
            }
        }

        public void SelectItem(string text)
        {
            if (text == currentText)
            {
                return;
            }
            if (listBox == null)
            {
                ApplyTemplate();
            }

            if (IsFiltering)
            {
                SelectItemFiltering(text);
            }
            else
            {
                SelectItemWithStart(text);
            }
            currentText = text;
        }

        private void SelectItemFiltering(string query)
        {
            var listToFilter = (currentList != null && (!string.IsNullOrEmpty(currentText)) && (!string.IsNullOrEmpty(query)) &&
                                query.StartsWith(currentText, StringComparison.Ordinal))
                ? currentList
                : completionData;

            var matchingItems =
                from item in listToFilter
                let quality = GetMatchQuality(item.Text, query)
                where quality > 0
                select new
                {
                    Item = item,
                    Quality = quality
                };

            var suggestedItem = listBox.SelectedIndex != -1 ? (ICompletionData)(listBox.Items[listBox.SelectedIndex]) : null;

            var listBoxItems = new ObservableCollection<ICompletionData>();
            int bestIndex = -1;
            int bestQuality = -1;
            double bestPriority = 0;
            int i = 0;
            foreach (var matchingItem in matchingItems)
            {
                double priority = matchingItem.Item == suggestedItem ? double.PositiveInfinity : matchingItem.Item.Priority;
                int quality = matchingItem.Quality;
                if (quality > bestQuality || (quality == bestQuality && (priority > bestPriority)))
                {
                    bestIndex = i;
                    bestPriority = priority;
                    bestQuality = quality;
                }
                listBoxItems.Add(matchingItem.Item);
                i++;
            }
            currentList = listBoxItems;
            listBox.ItemsSource = listBoxItems;
            SelectIndexCentered(bestIndex);
        }

        private void SelectItemWithStart(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return;
            }

            int suggestedIndex = listBox.SelectedIndex;

            int bestIndex = -1;
            int bestQuality = -1;
            double bestPriority = 0;
            for (int i = 0; i < completionData.Count; ++i)
            {
                int quality = GetMatchQuality(completionData[i].Text, query);
                if (quality < 0)
                {
                    continue;
                }

                double priority = completionData[i].Priority;
                bool useThisItem;
                if (bestQuality < quality)
                {
                    useThisItem = true;
                }
                else
                {
                    if (bestIndex == suggestedIndex)
                    {
                        useThisItem = false;
                    }
                    else if (i == suggestedIndex)
                    {
                        useThisItem = bestQuality == quality;
                    }
                    else
                    {
                        useThisItem = bestQuality == quality && bestPriority < priority;
                    }
                }
                if (useThisItem)
                {
                    bestIndex = i;
                    bestPriority = priority;
                    bestQuality = quality;
                }
            }
            SelectIndexCentered(bestIndex);
        }

        private void SelectIndexCentered(int bestIndex)
        {
            if (bestIndex < 0)
            {
                listBox.ClearSelection();
            }
            else
            {
                int firstItem = listBox.FirstVisibleItem;
                if (bestIndex < firstItem || firstItem + listBox.VisibleItemCount <= bestIndex)
                {
                    listBox.CenterViewOn(bestIndex);
                    listBox.SelectIndex(bestIndex);
                }
                else
                {
                    listBox.SelectIndex(bestIndex);
                }
            }
        }

        private int GetMatchQuality(string itemText, string query)
        {
            if (itemText == null)
            {
                throw new ArgumentNullException("itemText", "ICompletionData.Text returned null");
            }

            if (query == itemText)
            {
                return 8;
            }
            if (string.Equals(itemText, query, StringComparison.InvariantCultureIgnoreCase))
            {
                return 7;
            }

            if (itemText.StartsWith(query, StringComparison.InvariantCulture))
            {
                return 6;
            }
            if (itemText.StartsWith(query, StringComparison.InvariantCultureIgnoreCase))
            {
                return 5;
            }

            bool? camelCaseMatch = null;
            if (query.Length <= 2)
            {
                camelCaseMatch = CamelCaseMatch(itemText, query);
                if (camelCaseMatch == true)
                {
                    return 4;
                }
            }

            if (IsFiltering)
            {
                if (itemText.IndexOf(query, StringComparison.InvariantCulture) >= 0)
                {
                    return 3;
                }
                if (itemText.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    return 2;
                }
            }

            if (!camelCaseMatch.HasValue)
            {
                camelCaseMatch = CamelCaseMatch(itemText, query);
            }
            if (camelCaseMatch == true)
            {
                return 1;
            }

            return -1;
        }

        private static bool CamelCaseMatch(string text, string query)
        {
            var theFirstLetterOfEachWord = text.Take(1).Concat(text.Skip(1).Where(char.IsUpper));

            int i = 0;
            foreach (var letter in theFirstLetterOfEachWord)
            {
                if (i > query.Length - 1)
                {
                    return true;
                }
                if (char.ToUpperInvariant(query[i]) != char.ToUpperInvariant(letter))
                {
                    return false;
                }
                i++;
            }
            if (i >= query.Length)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}