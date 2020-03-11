using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement
{
    public class ApplicationUser:IdentityUser
    {
        public string City { get; set; }
    }
}