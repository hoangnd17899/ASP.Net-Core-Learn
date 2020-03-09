using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name="Confirm Password")]
        [Compare("Password",ErrorMessage="Do not match with Password")]
        public string ConfirmPassword { get; set; }
    }
}