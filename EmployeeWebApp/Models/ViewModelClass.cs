using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace EmployeeWebApp.Models
{
    public class ViewModelClass
    {
        [DataMember(Name = "Employee_Id")]
        public int? Employee_Id { get; set; }
        [DataMember(Name = "Name")]
        public string? Name { get; set; }
        [DataMember(Name = "Age")]
        [Range(1, 100)]
        public int? Age { get; set; }
        [DataMember(Name = "Qualification")]
        public string? Qualification { get; set; }
        [DataMember(Name = "Address")]
        public string? Address { get; set; }
        [DataMember(Name = "Department")]
        public string Department { get; set; }
        public List<DepartmentClass> Departments { get; set; }

    }
}
