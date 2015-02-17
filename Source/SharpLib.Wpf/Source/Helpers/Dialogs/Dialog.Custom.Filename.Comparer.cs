﻿using System.Collections.Generic;

namespace SharpLib.Wpf.Dialogs
{
    /// <summary>
    /// Сравнение файлов (по расширение и по имени)
    /// </summary>
    internal class DialogCustomFilenameComparer : IComparer<string>
    {
        #region Методы

        public int Compare(string x, string y)
        {
            var ext1 = Files.GetExtension(x);
            var ext2 = Files.GetExtension(y);
            var name1 = Files.GetFileName(x);
            var name2 = Files.GetFileName(y);

            if (ext1.EqualsOrdinalEx(ext2) == false)
            {
                return ext1.CompareToEx(ext2);
            }

            return name1.CompareToEx(name2);
        }

        #endregion
    }
}