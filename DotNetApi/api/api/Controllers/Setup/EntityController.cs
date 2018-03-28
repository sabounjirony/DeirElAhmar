using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL.Setup;
using Api.Filters;
using VM.Setup;

namespace Api.Controllers.Setup
{
    [ApiAspect]
    public class EntityController : ApiController
    {
        private const string RouteName = "Entity";
        private readonly BlEntity _blObj = new BlEntity();

        [HttpGet, Route(RouteName), CustomAuthorizationFilter]
        public HttpResponseMessage LoadPaging(string _search, long rows, int page, string sidx, string sord)
        {
            var toRet = _blObj.LoadPaging(ApiCommon.CurrentUserId, _search, page, out rows, sidx, sord);
            var response = Request.CreateResponse(HttpStatusCode.OK, toRet);
            return response;
        }

        [HttpGet, Route(RouteName), CustomAuthorizationFilter]
        public HttpResponseMessage Init(long id)
        {
            var toRet = _blObj.Init(ApiCommon.CurrentUserId, id);
            var response = Request.CreateResponse(HttpStatusCode.OK, toRet);
            return response;
        }

        [HttpPost, Route(RouteName), CustomAuthorizationFilter]
        public void Save(EntityVm model)
        { _blObj.Save(ApiCommon.CurrentUserId, model); }

        [HttpGet, Route(RouteName), CustomAuthorizationFilter]
        public void Delete(long id)
        { _blObj.Delete(ApiCommon.CurrentUserId, id);}
    }
}