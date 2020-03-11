using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        // Sử dụng Remote gọi đến hàm kiểm tra xem có trùng với email đã đăng ký hay không, trả về validate bằng json
        [Remote("IsEmailInUse","Account")]
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