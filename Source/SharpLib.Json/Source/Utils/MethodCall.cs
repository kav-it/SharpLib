namespace SharpLib.Json
{
    internal delegate TResult MethodCall<in T, out TResult>(T target, params object[] args);
}