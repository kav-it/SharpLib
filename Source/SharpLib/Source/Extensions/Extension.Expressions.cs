using System.Linq.Expressions;
using System.Reflection;

namespace SharpLib
{
    /// <summary>
    /// Класс расширения для "Expression"
    /// </summary>
    public static class ExtensionExpression
    {
        #region Методы

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member;
        }

        #endregion
    }
}