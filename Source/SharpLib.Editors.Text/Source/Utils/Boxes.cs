namespace SharpLib.Notepad.Utils
{
    internal static class Boxes
    {
        #region Поля

        public static readonly object False = false;

        public static readonly object True = true;

        #endregion

        #region Методы

        public static object Box(bool value)
        {
            return value ? True : False;
        }

        #endregion
    }
}