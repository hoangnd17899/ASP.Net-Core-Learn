using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace EmployeeManagement
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name="Remember me")]
        public bool RememberMe { get; set; }

        // Lưu url khi chuyển hướng đến trang đăng nhập google
        public string ReturnUrl { get; set; }
        // Lưu các thông tin đăng nhập bên ngoài
        public IList<AuthenticationScheme> ExternalLogins {get;set;}
    }
}