using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement
{
    public class EmployeeCreateViewModel
    {
        [Required]
        [MaxLength(30,ErrorMessage="Name cannot exceed 50 characters")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage="Invalid Email Format!")]
        [Display(Name="Office Mail")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public IFormFile Photo{ get; set; }
    }
}
