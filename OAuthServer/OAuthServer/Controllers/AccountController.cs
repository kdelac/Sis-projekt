namespace OAuthServer.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using OAuthServer.Models;

    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type,
            string client_id,
            string redirect_uri,
            string scope,
            string state)
        {
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri);
            query.Add("state", state);

            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username,
            string password,
            string redirectUri,
            string state)
        {
            var user = Users.GetAll().Find(_ =>_.UserName == username && _.Password == password);
            if (user != null)
            {
                const string code = "KONSTANTA";
                var query = new QueryBuilder();
                query.Add("code", code);
                query.Add("state", state);
                
                return Redirect($"{redirectUri}{query.ToString()}");
            }


            return Redirect($"{redirectUri}");
        }
        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id,
            string refresh_token)
        {
            var claims = new[]
          {
                new Claim(JwtRegisteredClaimNames.Sub, "id"),
                new Claim("claim", "cookie")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Config.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Config.Issuer,
                Config.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: grant_type == "refresh_token"
                    ? DateTime.Now.AddMinutes(5)
                    : DateTime.Now.AddMilliseconds(1),
                signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "sisLabosi",
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);
        }

        [Authorize]
        public IActionResult Validate()
        {
            if (HttpContext.Request.Query.TryGetValue("access_token", out var accessToken))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}