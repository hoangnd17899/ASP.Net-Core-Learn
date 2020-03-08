using System;
using System.Collections.Generic;

namespace EmployeeManagement
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int id);
        IEnumerable<Employee> GetAllEmployee();
        Employee Add(Employee emp);
        Employee Update(Employee emp);
        Employee Delete(int id);
    }
}
