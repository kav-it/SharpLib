using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpLib.Notepad.Utils
{
    internal interface IFreezable
    {
        #region Свойства

        bool IsFrozen { get; }

        #endregion

        #region Методы

        void Freeze();

        #endregion
    }

    internal static class FreezableHelper
    {
        #region Методы

        public static void ThrowIfFrozen(IFreezable freezable)
        {
            if (freezable.IsFrozen)
            {
                throw new InvalidOperationException("Cannot mutate frozen " + freezable.GetType().Name);
            }
        }

        public static IList<T> FreezeListAndElements<T>(IList<T> list)
        {
            if (list != null)
            {
                foreach (T item in list)
                {
                    Freeze(item);
                }
            }
            return FreezeList(list);
        }

        public static IList<T> FreezeList<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return Empty<T>.Array;
            }
            if (list.IsReadOnly)
            {
                return list;
            }
            return new ReadOnlyCollection<T>(list.ToArray());
        }

        public static void Freeze(object item)
        {
            var f = item as IFreezable;
            if (f != null)
            {
                f.Freeze();
            }
        }

        public static T FreezeAndReturn<T>(T item) where T : IFreezable
        {
            item.Freeze();
            return item;
        }

        public static T GetFrozenClone<T>(T item) where T : IFreezable, ICloneable
        {
            if (!item.IsFrozen)
            {
                item = (T)item.Clone();
                item.Freeze();
            }
            return item;
        }

        #endregion
    }

    [Serializable]
    internal abstract class AbstractFreezable : IFreezable
    {
        #region Поля

        private bool isFrozen;

        #endregion

        #region Свойства

        public bool IsFrozen
        {
            get { return isFrozen; }
        }

        #endregion

        #region Методы

        public void Freeze()
        {
            if (!isFrozen)
            {
                FreezeInternal();
                isFrozen = true;
            }
        }

        protected virtual void FreezeInternal()
        {
        }

        #endregion
    }
}