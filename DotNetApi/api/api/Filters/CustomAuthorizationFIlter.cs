using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BL.System.Logging;
using BL.System.Security;
using Api;

namespace api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    [ApiAspect]
    public class CustomAuthorizationFilter : AuthorizationFilterAttribute
    {
        readonly bool _active;

        public CustomAuthorizationFilter(bool active = true)
        { _active = active; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (_active)
            {
                var userId = ParseAuthorizationHeader(actionContext);

                //Special case for logout with no ticket
                if (userId == null)
                {
                    if (actionContext.Request.RequestUri.AbsolutePath.ToUpper().Contains("LOGOUT"))
                    { userId = 1; }
                    else
                    { throw new BusinessException("InvalidTicket", "lblUser"); }
                }
                apiCommon.CurrentUserId = (long)userId;
                base.OnAuthorization(actionContext);
            }
        }

        protected virtual long? ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authHeader = null;
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Token")
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader)) return null;

            var blUser = new BlUser();
            var userId = blUser.Authorize(authHeader, actionContext.Request.RequestUri.ToString());
            return userId;
        }
    }
}
