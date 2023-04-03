using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System;
using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EmployeeWebApp.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web.Providers.Entities;

namespace EmployeeWebApp.Controllers
{
    public class LoginController : Controller
    {
        readonly string Baseurl = "http://localhost:55440/api/Account/";
        private readonly AuthApiClient _apiClient;
        public LoginController(AuthApiClient apiClient, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(SignUpModel signUpModel)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(Baseurl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string jsonobject = JsonConvert.SerializeObject(signUpModel);
            StringContent stream = new StringContent(jsonobject, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(Baseurl + "login", stream);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Welcome",signUpModel);
            }
            else if (result.StatusCode.Equals(401))
            {
                return View(2); 
            }
            else
                return View();
        }

        [HttpGet]
        public ActionResult Welcome(SignUpModel signUpModel, string result)
        {
            if (string.IsNullOrEmpty(signUpModel.Email))
                return View("Login", "Login First !!!");
            ViewData["UserName"] = (signUpModel.FirstName+" "+signUpModel.LastName);
            return View(signUpModel);
        }

        public IActionResult Logout()
        {
            // delete session for user
            HttpContext.Session.Remove("AuthToken");
            return RedirectToAction("Index");
        }
    }
}
