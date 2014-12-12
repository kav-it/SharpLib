using System;

namespace SharpLib
{
    public static class ExtensionGuid
    {
        public static string ToTokenEx(this Guid self)
        {
            return self.ToByteArray().ToAsciiEx(string.Empty).ToLower();
        }
    }
}