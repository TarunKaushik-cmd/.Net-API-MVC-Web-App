using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using ActionNameAttribute = System.Web.Mvc.ActionNameAttribute;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace EmployeeWebApp.Controllers
{
    public class SignUpController : Controller
    {
        readonly string Baseurl = "http://192.168.29.103:55440/api/Account/";
        public SignUpController() { }
        
        public IActionResult Index()
        {
            ViewData["result"] = TempData["Status"];
            TempData["Status"] = 0;
            return View();
 
        }

        [HttpPost]
        public async Task<ActionResult> IndexAsync(SignUpModel signUpModel)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(Baseurl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string jsonobject = JsonConvert.SerializeObject(signUpModel);
            StringContent stream = new StringContent(jsonobject, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(Baseurl + "signup", stream);
            if (result.IsSuccessStatusCode)
            {
                TempData["Status"] = 1;
                return RedirectToAction("Index","Login");
            }
            if ((int)result.StatusCode==400)
            {
                TempData["Status"] = 400;
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");
        }
    }
}
