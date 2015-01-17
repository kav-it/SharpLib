using System;

namespace SharpLib
{
    /// <summary>
    /// Расширение класса "Guid"
    /// </summary>
    public static class ExtensionGuid
    {
        /// <summary>
        /// Генерация токена из Guid
        /// </summary>
        /// <remarks>
        /// DB3E4A8D-23E8-4E46-AD6F-7365C862E5A7 = db3e4a8d23e84e46ad6f7365c862e5a7
        /// </remarks>
        public static string ToTokenEx(this Guid self)
        {
            return self.ToByteArray().ToAsciiEx(string.Empty).ToLower();
        }
    }
}