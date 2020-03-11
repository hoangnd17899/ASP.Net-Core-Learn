using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}