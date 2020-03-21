using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement
{
    public class Employee
    {
        public int Id { get; set; }
        [NotMappedAttribute]
        public string EncryptedId { get; set; }
        [Required]
        [MaxLength(30,ErrorMessage="Name cannot exceed 50 characters")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage="Invalid Email Format!")]
        [Display(Name="Office Mail")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public string PhotoPath{ get; set; }
    }
}
