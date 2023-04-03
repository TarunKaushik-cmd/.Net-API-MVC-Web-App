﻿using EmployeeWebApp.Models;
using EmployeeWebApp.Services;
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
        private readonly AuthApiClient _apiClient;
        public SignUpController(AuthApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        readonly string Baseurl = "http://localhost:55440/api/Account/";
        public IActionResult Index()
        {
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
                return View(1);
            }
            else
                return View();
        }
    }
}