using System;

namespace SharpLib
{
    public static class ExtensionException
    {
        /// <summary>
        /// Чтение актуального внутреннего текста исключений
        /// </summary>
        public static string GetMessageEx(this Exception self)
        {
            while (self.InnerException != null)
            {
                self = self.InnerException;
            }

            return self.Message;
        }
    }
}