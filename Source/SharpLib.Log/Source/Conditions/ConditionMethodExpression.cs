using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

using NLog.Common;

namespace NLog.Conditions
{
    internal sealed class ConditionMethodExpression : ConditionExpression
    {
        #region Поля

        private readonly bool acceptsLogEvent;

        private readonly string conditionMethodName;

        #endregion

        #region Свойства

        public MethodInfo MethodInfo { get; private set; }

        public IList<ConditionExpression> MethodParameters { get; private set; }

        #endregion

        #region Конструктор

        public ConditionMethodExpression(string conditionMethodName, MethodInfo methodInfo, IEnumerable<ConditionExpression> methodParameters)
        {
            MethodInfo = methodInfo;
            this.conditionMethodName = conditionMethodName;
            MethodParameters = new List<ConditionExpression>(methodParameters).AsReadOnly();

            ParameterInfo[] formalParameters = MethodInfo.GetParameters();
            if (formalParameters.Length > 0 && formalParameters[0].ParameterType == typeof(LogEventInfo))
            {
                acceptsLogEvent = true;
            }

            int actualParameterCount = MethodParameters.Count;
            if (acceptsLogEvent)
            {
                actualParameterCount++;
            }

            int requiredParametersCount = 0;
            int optionalParametersCount = 0;

            foreach (var param in formalParameters)
            {
                if (param.IsOptional)
                {
                    ++optionalParametersCount;
                }
                else
                {
                    ++requiredParametersCount;
                }
            }

            if (!((actualParameterCount >= requiredParametersCount) && (actualParameterCount <= formalParameters.Length)))
            {
                string message;

                if (optionalParametersCount > 0)
                {
                    message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Condition method '{0}' requires between {1} and {2} parameters, but passed {3}.",
                        conditionMethodName,
                        requiredParametersCount,
                        formalParameters.Length,
                        actualParameterCount);
                }
                else
                {
                    message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Condition method '{0}' requires {1} parameters, but passed {2}.",
                        conditionMethodName,
                        requiredParametersCount,
                        actualParameterCount);
                }
                InternalLogger.Error(message);
                throw new ConditionParseException(message);
            }
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(conditionMethodName);
            sb.Append("(");

            string separator = string.Empty;
            foreach (ConditionExpression expr in MethodParameters)
            {
                sb.Append(separator);
                sb.Append(expr);
                separator = ", ";
            }

            sb.Append(")");
            return sb.ToString();
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            int parameterOffset = acceptsLogEvent ? 1 : 0;

            var callParameters = new object[MethodParameters.Count + parameterOffset];
            int i = 0;
            foreach (ConditionExpression ce in MethodParameters)
            {
                callParameters[i++ + parameterOffset] = ce.Evaluate(context);
            }

            if (acceptsLogEvent)
            {
                callParameters[0] = context;
            }

            return MethodInfo.DeclaringType.InvokeMember(
                MethodInfo.Name,
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.OptionalParamBinding,
                null,
                null,
                callParameters
                , CultureInfo.InvariantCulture
                );
        }

        #endregion
    }
}