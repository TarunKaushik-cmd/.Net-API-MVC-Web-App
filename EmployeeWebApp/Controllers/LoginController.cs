using Microsoft.AspNetCore.Mvc;
using System;
using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EmployeeWebApp.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace EmployeeWebApp.Controllers
{
    public class LoginController : Controller
    {
        readonly string Baseurl = "http://localhost:55440/api/Account/";
        private readonly AuthApiClient _apiClient;
        public LoginController(AuthApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public IActionResult Index()
        {
            ViewData["result"] = TempData["Status"];
            TempData["Status"]=0;
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
                var responseContent = await result.Content.ReadAsStringAsync();
                var token= JsonConvert.DeserializeObject<dynamic>(responseContent);
                TempData.Remove("Email");
                TempData.Add("Email", signUpModel.Email.ToString());
                // Create a new cookie with a unique name for the JWT token
                HttpContext.Response.Cookies.Append(
                    "token", token,
                    new Microsoft.AspNetCore.Http.CookieOptions { 
                        Expires = DateTime.Now.AddHours(1),
                        HttpOnly= true,
                        Secure =true,
                        IsEssential = true
                    });
                return RedirectToAction("Welcome");
            }
            else if (result.StatusCode.Equals(401))
            {
                TempData["Status"]=2;
                return RedirectToAction("Index"); 
            }
            else
                return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Welcome(string email)
        {
            SignUpModel signUpModel = new SignUpModel();
            if (HttpContext.Request.Cookies.TryGetValue("token", out string token))
            {
                if (!HttpContext.Request.Cookies.TryGetValue("UserName", out string _userName))
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());
                    HttpResponseMessage httpResponseMessage = await client.GetAsync("user" + string.Format("?email={0}", TempData["Email"].ToString()));
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        signUpModel = JsonConvert.DeserializeObject<SignUpModel>(result);
                        _userName = signUpModel.FirstName + " " + signUpModel.LastName;
                        HttpContext.Response.Cookies.Append(
                            "UserName", _userName, 
                            new Microsoft.AspNetCore.Http.CookieOptions { 
                                Expires = DateTime.Now.AddHours(1),
                                IsEssential = true 
                            });
                        return View(signUpModel);
                    }
                }
                else
                {
                    HttpContext.Request.Cookies.TryGetValue("UserName", out _userName);
                    signUpModel.FirstName = _userName.Substring(0, _userName.IndexOf(" "));
                    signUpModel.LastName = _userName.Substring(_userName.IndexOf(" ") + 1, _userName.Length - _userName.IndexOf(" ") - 1);
                    return View(signUpModel);
                }
            }
            else
            {
                TempData["Status"] = 3;
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Logout()
        {
            // delete session for user
            HttpContext.Response.Cookies.Delete("token");
            HttpContext.Response.Cookies.Delete("UserName");
            TempData.Remove("Email");
            return RedirectToAction("Index");
        }
    }
}
