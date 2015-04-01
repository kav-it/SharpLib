using System;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Utils
{
    internal static class BusyManager
    {
        #region Поля

        [ThreadStatic]
        private static List<object> _activeObjects;

        #endregion

        #region Методы

        public static BusyLock Enter(object obj)
        {
            var activeObjects = _activeObjects;
            if (activeObjects == null)
            {
                activeObjects = _activeObjects = new List<object>();
            }
            for (int i = 0; i < activeObjects.Count; i++)
            {
                if (activeObjects[i] == obj)
                {
                    return BusyLock.Failed;
                }
            }
            activeObjects.Add(obj);
            return new BusyLock(activeObjects);
        }

        #endregion

        #region Вложенный класс: BusyLock

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Should always be used with 'var'")]
        public struct BusyLock : IDisposable
        {
            #region Поля

            public static readonly BusyLock Failed = new BusyLock(null);

            private readonly List<object> objectList;

            #endregion

            #region Свойства

            public bool Success
            {
                get { return objectList != null; }
            }

            #endregion

            #region Конструктор

            internal BusyLock(List<object> objectList)
            {
                this.objectList = objectList;
            }

            #endregion

            #region Методы

            public void Dispose()
            {
                if (objectList != null)
                {
                    objectList.RemoveAt(objectList.Count - 1);
                }
            }

            #endregion
        }

        #endregion
    }
}