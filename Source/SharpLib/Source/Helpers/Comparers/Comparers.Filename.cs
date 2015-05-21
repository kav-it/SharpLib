using System;
using System.Collections.Generic;

namespace SharpLib
{
    /// <summary>
    /// Сравнение по имени файла с учетом расширения
    /// </summary>
    public class ComparersFilename : IComparer<string>
    {
        #region Методы

        public int Compare(string x, string y)
        {
            var ext1 = Files.GetExtension(x);
            var ext2 = Files.GetExtension(y);
            var name1 = Files.GetFileName(x);
            var name2 = Files.GetFileName(y);

            if (name1.IsNotValid() && ext1.IsValid())
            {
                name1 = "." + ext1;
                ext1 = string.Empty;
            }

            if (name2.IsNotValid() && ext2.IsValid())
            {
                name2 = "." + ext2;
                ext2 = string.Empty;
            }

            if (ext1.EqualsOrdinalEx(ext2) == false)
            {
                return ext1.CompareToEx(ext2, StringComparison.OrdinalIgnoreCase);
            }

            var r = name1.CompareToEx(name2, StringComparison.OrdinalIgnoreCase);

            return r;
        }

        #endregion
    }
}