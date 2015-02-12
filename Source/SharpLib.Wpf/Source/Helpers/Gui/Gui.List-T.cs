using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace SharpLib.Wpf
{
    public class GuiList<T> : ObservableCollection<T>
    {
        #region Конструктор

        public GuiList()
        {
        }

        public GuiList(List<T> list)
            : base(list)
        {
        }

        public GuiList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        #endregion

        #region Методы

        private void ApplySort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, ListSortDirection direction)
        {
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    {
                        ApplySort(Items.OrderBy(keySelector));
                        break;
                    }
                case ListSortDirection.Descending:
                    {
                        ApplySort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            ApplySort(Items.OrderBy(keySelector, comparer));
        }

        public void AddSafety(T value)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    (Action)(() => { Add(value); })
                    );
            }
        }

        public List<T> ToList()
        {
            List<T> result = new List<T>(Items);

            return result;
        }

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }

            //CheckReentrancy();
            //int startingIndex = Count;
            //foreach (var data in collection)
            //{
            //    Items.Add(data);
            //}
            //OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            //OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            //var changedItems = new List<T>(collection);

            //var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startingIndex);
            //this.OnCollectionChanged(args);
        }

        public void MoveEx(int newIndex, T value)
        {
            if (newIndex < 0 || newIndex > Count)
            {
                return;
            }

            var oldIndex = IndexOf(value);

            if (oldIndex == -1)
            {
                return;
            }

            if (newIndex == Count)
            {
                // Добавление к конец списка
                RemoveAt(oldIndex);
                Add(value);
            }
            else
            {
                RemoveAt(oldIndex);

                if (newIndex > oldIndex)
                {
                    newIndex--;
                }

                Insert(newIndex, value);
            }
        }

        #endregion
    }
}