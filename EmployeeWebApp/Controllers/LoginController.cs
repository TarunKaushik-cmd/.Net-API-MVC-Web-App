using Microsoft.AspNetCore.Mvc;
using System;
using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace EmployeeWebApp.Controllers
{
    public class LoginController : Controller
    {
        readonly string Baseurl = "http://192.168.29.103:55440/api/Account/";
        public LoginController() { }
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
            var result =  await client.PostAsync(Baseurl + "login", stream);
            var responseContent = await result.Content.ReadAsStringAsync();
            responseContent = responseContent.Substring(1,responseContent.Length-2);
            if (result.IsSuccessStatusCode)
            {
                responseContent = await result.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<dynamic>(responseContent);
                TempData.Remove("Email");
                TempData.Add("Email", signUpModel.Email.ToString());
                // Create a new cookie with a unique name for the JWT token
                HttpContext.Response.Cookies.Append(
                    "token", token,
                    new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(1),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite=SameSiteMode.Lax
                    });
                TempData["Status"] = 0;
                return RedirectToAction("Welcome");
            }
            else if (responseContent.Trim('/') == "Wrong Password")
            {
                TempData["Status"] = 401;
                return RedirectToAction("Index");
            }
            else if (responseContent.Trim('/') =="Wrong Email Or User Does not exist")
            {
                TempData["Status"] = 402;
                return RedirectToAction("Index");
            }
            else
                TempData["Status"] = 405;
                return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Welcome(string email)
        {
            if (HttpContext.Request.Cookies.TryGetValue("token", out string token))
            {
                SignUpModel signUpModel = new SignUpModel();
                if (HttpContext.Request.Cookies.TryGetValue("UserName", out string _userName))
                {
                    signUpModel.FirstName = _userName.Substring(0, _userName.IndexOf(" "));
                    signUpModel.LastName = _userName.Substring(_userName.IndexOf(" ") + 1, _userName.Length - _userName.IndexOf(" ") - 1);
                    return View(signUpModel);
                }
                else
                {
                    using var client = new HttpClient();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());
                    var httpResponseMessage = await client.GetAsync("user" + string.Format("?email={0}", TempData["Email"].ToString()));
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        signUpModel = JsonConvert.DeserializeObject<SignUpModel>(result);
                        _userName = signUpModel.FirstName + " " + signUpModel.LastName;
                        HttpContext.Response.Cookies.Append(
                            "UserName", _userName,
                            new Microsoft.AspNetCore.Http.CookieOptions
                            {
                                Expires = DateTime.Now.AddHours(1),
                                IsEssential = true,
                                HttpOnly = true,
                                Secure = true
                            });
                        return View(signUpModel);
                    }
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
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Expires"] = "-1";
            Response.Headers["Pragma"] = "no-cache";
            HttpContext.Response.Cookies.Delete("token");
            HttpContext.Response.Cookies.Delete("UserName");
            TempData.Remove("Email");
            return RedirectToAction("Index");
        }
    }
}
