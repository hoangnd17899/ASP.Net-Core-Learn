using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;

namespace EmployeeManagement
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowDomain;

        public ValidEmailDomainAttribute(string allowDomain)
        {
            this.allowDomain=allowDomain;
        }

        public override bool IsValid(object value){
            string domain=value.ToString().Split('@').ToArray()[1];
            return domain.ToUpper()==allowDomain.ToUpper();
        }
    }
}