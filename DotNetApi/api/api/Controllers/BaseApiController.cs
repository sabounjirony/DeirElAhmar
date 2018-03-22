using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace api.Controllers
{
    public class BaseApiController : ApiController
    {
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}