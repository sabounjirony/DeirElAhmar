using System;
using System.Collections.Generic;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        // GET: api/User
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        // GET: api/User/5
        public string Get(long id)
        {
            return "{value : " + id + "}";
        }

        [HttpPost]
        // POST: this is used for add
        public string Search(Filter filter)
        {
            return "{search: " + filter.term + "}";
        }

        // PUT: this is used for update
        //public void Put(int id, User value)
        //{

        //}

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        // Authenticate: api/User/5
        public string Authenticate(Credentials userCredentials)
        {
            // Define const Key this should be private secret key stored in some safe place
            //string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            byte[] bytes = Encoding.UTF8.GetBytes("D:jer9g[<KYx@Z2,n;u]\"!'m{z=&/aW`P-HRF4A(dL%+vJ~6$f");
            string key = Convert.ToBase64String(bytes);

            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //  Finally create a Token
            var header = new JwtHeader(credentials);

            //Some PayLoad that contain information about the  customer
            var payload = new JwtPayload
           {
               { "userName ", userCredentials.UserName},
               { "id", 1},
               { "validity", DateTime.Now.AddHours(2).ToString("yyyyMMdd hh:mm:ss tt")}
           };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);

            return tokenString;
        }

        private bool Validate(string token)
        {
            return false;
        }
    }
}

public class Credentials
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class Filter
{
    public string term { get; set; }
}