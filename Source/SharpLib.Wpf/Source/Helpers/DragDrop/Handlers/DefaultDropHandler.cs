using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using SharpLib.Wpf.Dragging.Utilities;

namespace SharpLib.Wpf.Dragging
{
    public class DefaultDropHandler : IDropTarget
    {
        #region Методы

        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        public virtual void Drop(IDropInfo dropInfo)
        {
            var insertIndex = dropInfo.InsertIndex != dropInfo.UnfilteredInsertIndex ? dropInfo.UnfilteredInsertIndex : dropInfo.InsertIndex;
            var destinationList = dropInfo.TargetCollection.ToList();
            var data = ExtractData(dropInfo.Data);

            var enumerable = data as IList<object> ?? data.Cast<object>().ToList();
            if (Equals(dropInfo.DragInfo.VisualSource, dropInfo.VisualTarget))
            {
                var sourceList = dropInfo.DragInfo.SourceCollection.ToList();

                foreach (var o in enumerable)
                {
                    var index = sourceList.IndexOf(o);

                    if (index != -1)
                    {
                        sourceList.RemoveAt(index);

                        if (Equals(sourceList, destinationList) && index < insertIndex)
                        {
                            --insertIndex;
                        }
                    }
                }
            }

            foreach (var o in enumerable)
            {
                destinationList.Insert(insertIndex++, o);
            }
        }

        public static bool CanAcceptData(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return false;
            }

            if (!dropInfo.IsSameDragDropContextAsSource)
            {
                return false;
            }

            if (Equals(dropInfo.DragInfo.SourceCollection, dropInfo.TargetCollection))
            {
                return dropInfo.TargetCollection.ToList() != null;
            }
            if (dropInfo.DragInfo.SourceCollection is ItemCollection)
            {
                return false;
            }
            if (dropInfo.TargetCollection == null)
            {
                return false;
            }
            if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
            {
                return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
            }
            return false;
        }

        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }
            return Enumerable.Repeat(data, 1);
        }

        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (Equals(parent, sourceItem))
                {
                    return true;
                }

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) => (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
            var enumerableTypes = from i in enumerableInterfaces
                                  select i.GetGenericArguments().Single();

            var enumerable = enumerableTypes as IList<Type> ?? enumerableTypes.ToList();
            if (enumerable.Any())
            {
                var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerable.Any(t => t.IsAssignableFrom(dataType));
            }
            return target is IList;
        }

        #endregion
    }
}