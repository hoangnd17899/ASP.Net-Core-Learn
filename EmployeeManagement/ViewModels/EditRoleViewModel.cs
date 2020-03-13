using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            this.Users=new List<string>();
        }
        public string Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}