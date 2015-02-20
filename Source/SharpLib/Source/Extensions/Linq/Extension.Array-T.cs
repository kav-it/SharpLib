namespace SharpLib
{
    /// <summary>
    /// Класс расширения для "ExtensionArrayT"
    /// </summary>
    public static class ExtensionArrayT
    {
        public static int IndexOf<T>(this T[] self, T value) where T : class
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}