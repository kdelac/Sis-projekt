using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OAuthClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OAuthClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> SecretAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var splited = accessToken.Split(".");

            byte[] data = Convert.FromBase64String(splited[0]);
            string prvi = Encoding.UTF8.GetString(data);

            byte[] data2 = Convert.FromBase64String(splited[1]);
            string drugi = Encoding.UTF8.GetString(data2);

            AccesTokenViewModel accesTokenViewModel = new AccesTokenViewModel()
            {
                AccesToken = accessToken,
                Prvi = prvi,
                Drugi = drugi,
                Treci = "ne moze se dekodirat"
            };
            return View(accesTokenViewModel);
        }

        [Authorize]
        public IActionResult SignOut()
        {
            HttpContext.SignOutAsync();
            HttpContext.Response.Cookies.Delete("Kolacic");
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
