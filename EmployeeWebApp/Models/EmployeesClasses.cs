using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EmployeeWebApp.Models
{
    public class EmployeesClasses // for storing the result of the join
    {
        [DataMember(Name="Employee_Id")]
        public int Employee_Id { get; set; }
        [Required]
        [DataMember(Name = "Name")]
        public string? Name { get; set; }
        [Required]
        [DataMember(Name = "Age")]
        [Range(1,100)]
        public int? Age { get; set; }
        [Required]
        [DataMember(Name = "Qualification")]
        public string? Qualification { get; set; }
        [Required]
        [DataMember(Name = "Address")]
        public string? Address { get; set; }
        [Required]
        [DataMember(Name = "Department")]
        public string Department { get; set; }
    }
    public class DepartmentClass
    {
        public string DeptName { get; set; }
    }
    
}
