using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}