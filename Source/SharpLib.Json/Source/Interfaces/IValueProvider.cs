namespace SharpLib.Json
{
    public interface IValueProvider
    {
        #region Методы

        void SetValue(object target, object value);

        object GetValue(object target);

        #endregion
    }
}