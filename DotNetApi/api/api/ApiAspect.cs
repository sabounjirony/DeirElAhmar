using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api
{
    [Serializable]
    public class ApiAspect : OnMethodBoundaryAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            if (args.Exception.GetType() == typeof(BusinessException))
            {
                var businessException = (BusinessException)args.Exception;
                if (Convert.ToString(businessException.ExtraVariables) != "")
                    throw new Exception(businessException.ExceptionCode + "|=|" +
                                        Convert.ToString(businessException.ExtraVariables));

                throw new Exception(businessException.ExceptionCode);
            }
            throw args.Exception;
        }
    }

    public static class apiCommon
    {
        public static long CurrentUserId;
    }
}
