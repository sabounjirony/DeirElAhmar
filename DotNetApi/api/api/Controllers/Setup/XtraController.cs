using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL.Setup;
using Api.Filters;
using VM.Setup;

namespace Api.Controllers.Setup
{
    [ApiAspect]
    public class XtraController : ApiController
    {
        private const string RouteName = "Xtra";
        private readonly BlXtra _blObj = new BlXtra();

        [HttpGet, Route(RouteName), CustomAuthorizationFilter]
        public HttpResponseMessage LoadPaging(string _search, long rows, int page, string sidx, string sord)
        {
            var toRet = _blObj.LoadPaging(ApiCommon.CurrentUserId, _search, page, out rows, sidx, sord);
            var response = Request.CreateResponse(HttpStatusCode.OK, toRet);
            return response;
        }

        [HttpGet, Route(RouteName), CustomAuthorizationFilter]
        public HttpResponseMessage Init(string Object, long id, string property)
        {
            var toRet = _blObj.Init(ApiCommon.CurrentUserId, Object, id, property);
            var response = Request.CreateResponse(HttpStatusCode.OK, toRet);
            return response;
        }

        [HttpPost, Route(RouteName), CustomAuthorizationFilter]
        public HttpResponseMessage Save(XtraVm model)
        {
            var toRet = _blObj.Save(ApiCommon.CurrentUserId, model);
            var response = Request.CreateResponse(HttpStatusCode.OK, toRet);
            return response;
        }

        [HttpGet, Route(RouteName), CustomAuthorizationFilter]
        public void Delete(string Object, long id, string property)
        { _blObj.Delete(ApiCommon.CurrentUserId, Object, id, property); }
    }
}