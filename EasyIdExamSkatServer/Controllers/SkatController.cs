using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyIdExamSkatServer.model;
using JWT;
using JWT.Serializers;
using Microsoft.AspNetCore.Cors;
//using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace EasyIdExamSkatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkatController : ControllerBase
    {
        // GET: api/Skat
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Skat/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}
        protected string GetName(string token)
        {
            string secret = "example_key";


            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            var jsonString = decoder.Decode(token, secret, verify: true);
            dynamic json = JsonConvert.DeserializeObject(jsonString);

            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = "./SqliteDB.db";

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                //Read the newly inserted data:
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = $"SELECT debt FROM skat where email='{json.email}'";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var message = reader.GetString(0);
                        return message;
                    }
                }

                return null;
            }

            //var key = Encoding.ASCII.GetBytes(secret);
            //var handler = new JwtSecurityTokenHandler();
            //var validations = new TokenValidationParameters
            //{
            //    ValidateIssuerSigningKey = false,
            //    IssuerSigningKey = new SymmetricSecurityKey(key),
            //    ValidateIssuer = false,
            //    ValidateAudience = false
            //};
            //var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            //return claims.Identity.Name;
        }

        // POST: api/Skat
        [EnableCors]
        [HttpPost]
        [Route("Skat")]
        public IActionResult Post([FromBody] Token token)
        {
            var debt = GetName(token.Value);
            var json = JsonConvert.SerializeObject(new { debt });

            return Ok(json);
        }
    }
}
