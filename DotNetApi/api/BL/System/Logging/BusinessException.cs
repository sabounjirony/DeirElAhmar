using System;
using Tools;

namespace BL.System.Logging
{
    [Serializable]
    [BlAspect(AspectPriority = 2)]
    public class BusinessException: Exception
    {
        public Enumerations.ErrorType Type { get; set; }
        public Enumerations.ErrorSeverity Severity { get; set; }
        public string ExceptionCode { get; set; }
        public string ExtraVariables { get; set; }
        public BusinessException(string exceptionCode)
        {
            ExceptionCode = exceptionCode;
        }
        public BusinessException(string exceptionCode, string extraVariables)
        {
            ExceptionCode = exceptionCode;
            ExtraVariables = extraVariables;

        }
    }
}