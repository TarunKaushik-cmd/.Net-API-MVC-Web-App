using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace EmployeeWebApp.Controllers
{
    public class EmployeeController : Microsoft.AspNetCore.Mvc.Controller
    {
        readonly string Baseurl = "http://192.168.29.103:55440/";

        [HttpPost]
        public ActionResult Search(string txt)
        {
            TempData["param"] = txt;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Index(string sortOrder,bool request=false)
        {
            HttpContext.Request.Cookies.TryGetValue("UserName", out string _userName);
            ViewData["UserName"] = _userName;
            if (request)
            {
                var employees = await GetDataAsync("XML");
                employees = Sorter(employees, sortOrder);
                return View(employees);
            }
            else
                return View();
        }
        [HttpGet]
        public async Task<ActionResult> LoadData()
        {
            var res=await GetDataAsync();
            return Json(new {data=res},JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateAsync(ViewModelClass? viewModel)
        {
            HttpContext.Request.Cookies.TryGetValue("token", out string token);
            HttpResponseMessage Res = await ApiConnectAsync("Department", null, token);
            var temp1 = JsonConvert.DeserializeObject<List<DepartmentClass>>(Res.Content.ReadAsStringAsync().Result);
            ViewBag.departmentdrop = temp1.Select(x => new SelectListItem { Text = x.Departments, Value = x.Departments.ToString() }).ToList();
            return View(viewModel);
        }
        public async Task<ActionResult> CreateOrEdit(IFormCollection data)
        {
            HttpContext.Request.Cookies.TryGetValue("token", out string token);
            EmployeesClasses employee = new EmployeesClasses();
            int.TryParse(data["Employee_Id"], out int id);
            if (id != 0) employee.Employee_Id = id; else employee.Employee_Id = 0;
            employee.Name = data["Name"];
            employee.Address = data["Address"];
            employee.Age = int.Parse(data["Age"]);
            employee.Qualification = data["Qualification"];
            employee.Department = data["Department"];
            HttpResponseMessage res = await ApiConnectAsync("CreateOrEdit", employee,token);
            if (res.IsSuccessStatusCode)
            {
                return Content("<script>alert('Employee Added Successfully');</script>");
            }
            else
                return Content("<script>alert('Employee Cannot be Added');</script>");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            HttpContext.Request.Cookies.TryGetValue("token", out string token);
            HttpResponseMessage Res = await ApiConnectAsync("GetById", id,token);
            EmployeesClasses EmpInfo = JsonConvert.DeserializeObject<EmployeesClasses>(Res.Content.ReadAsStringAsync().Result);
            ViewModelClass viewModel = new ViewModelClass()
            {
                Employee_Id = EmpInfo.Employee_Id,
                Name = EmpInfo.Name,
                Address = EmpInfo.Address,
                Age = EmpInfo.Age,
                Qualification = EmpInfo.Qualification,
                Department = EmpInfo.Department,

            };
            return RedirectToAction("Create", viewModel);
        }
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("token", out string token);
            HttpResponseMessage httpRequestMessage = await ApiConnectAsync("Delete", id, token);
            if (httpRequestMessage.IsSuccessStatusCode)
            {
                return Json(new { sum = "Employee Deleted Successfully" });
            }
            else
            {
                return Json(new { sum = "Employee Cannot be Deleted" });
            }
        }
        private async Task<HttpResponseMessage> ApiConnectAsync(string request, dynamic? obj,string token="")
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(Baseurl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            switch (request)
            {
                case "Department": return await client.GetAsync(Baseurl + "api/Department/");
                case "CreateOrEdit":
                    string jsonobject = JsonConvert.SerializeObject(obj);
                    StringContent stream = new StringContent(jsonobject, Encoding.UTF8, "application/json");
                    return await client.PostAsync(Baseurl + "api/Employee/", stream);
                case "GetById": return await client.GetAsync("api/Employee/get" + string.Format("?id={0}", obj));
                case "GetByDept": return await client.GetAsync("api/Employee/get" + string.Format("?deptname={0}", obj));
                case "GetAll": return await client.GetAsync("api/Employee/get");
                case "Delete": return await client.DeleteAsync("api/Employee" + string.Format("?id={0}", obj));
                default: return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }
        private IEnumerable<EmployeesClasses> Sorter(IEnumerable<EmployeesClasses> empInfo,string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.AgeSortParm = sortOrder == "Age" ? "Age_desc" : "Age";
            ViewBag.QualificationSortParm = sortOrder == "Qualification" ? "Qualification_desc" : "Qualification";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "Address_desc" : "Address";
            ViewBag.DepartmentSortParm = sortOrder == "Department" ? "Department_desc" : "Department";
            empInfo = sortOrder switch
            {
                "Name_desc" => empInfo.OrderByDescending(s => s.Name),
                "Age" => empInfo.OrderBy(s => s.Age),
                "Age_desc" => empInfo.OrderByDescending(s => s.Age),
                "Qualification" => empInfo.OrderBy(s => s.Qualification),
                "Qualification_desc" => empInfo.OrderByDescending(s => s.Qualification),
                "Address" => empInfo.OrderBy(s => s.Address),
                "Address_desc" => empInfo.OrderByDescending(s => s.Address),
                "Department" => empInfo.OrderBy(s => s.Department),
                "Department_desc" => empInfo.OrderByDescending(s => s.Department),
                _ => empInfo.OrderBy(s => s.Name),
            };
            return empInfo;
        }
        private async Task<dynamic> GetDataAsync(string type="")
        {
            IEnumerable<EmployeesClasses> employees = new List<EmployeesClasses>();
            HttpContext.Request.Cookies.TryGetValue("token", out string token);
            string deptname = (string)TempData["param"];
            int.TryParse(deptname, out int id);
            HttpResponseMessage httpResponseMessage;
            if (id != 0)
            {
                httpResponseMessage = await ApiConnectAsync("GetById", id, token);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var employeeResponseJson = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    employees.ToList().Add(JsonConvert.DeserializeObject<EmployeesClasses>(employeeResponseJson));
                }
            }
            else if (!string.IsNullOrEmpty(deptname))
            {
                httpResponseMessage = await ApiConnectAsync("GetByDept", deptname, token);
            }
            else
            {
                httpResponseMessage = await ApiConnectAsync("GetAll", null, token);
            }
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var EmpResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                employees = JsonConvert.DeserializeObject<List<EmployeesClasses>>(EmpResponse);
            }
            if(type=="XML")
                return employees;
            else
                return JsonConvert.SerializeObject(employees);
        }
    }
}
