using System;
using BL.System.Logging;
using BL.System.Security;
using PostSharp.Aspects;
using Tools;
using Tools.Utilities;

namespace BL
{
    [Serializable]
    public class BlAspect : OnMethodBoundaryAspect
    {
        #region Members

        public string Module { get; set; }
        public string Permission { get; set; }

        #endregion Members

        #region Methods

        public override void OnEntry(MethodExecutionArgs args)
        {
            if (Module != null && Permission != null)
            { BlPermission.CanDo(Convert.ToInt64(args.Arguments[0]), Module, Permission); }
        }

        public override void OnException(MethodExecutionArgs args)
        {
            var userId = CheckEmpty.Numeric(args.Arguments[0]) == 0 ? Constants.SystemUser : CheckEmpty.Numeric(args.Arguments[0]);

            if (args.Exception.GetType() == typeof(BusinessException))
            {
                var businessException = (BusinessException)args.Exception;
                BlLogError.HandleError(userId, businessException, args.Method, false);
                throw businessException;
            }

            BlLogError.HandleError(userId, args.Exception, args.Method, false);
            throw args.Exception;
        }

        #endregion Methods
    }
}