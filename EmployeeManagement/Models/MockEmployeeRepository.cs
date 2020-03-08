using System;
using System.Linq;
using System.Collections.Generic;
using EmployeeManagement;

namespace EmployeeManagement
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>(){
                new Employee(){Id=1,Name="Mary",Department = Dept.HR,Email="mary@gmail.com"},
                new Employee(){Id=2,Name="John",Department = Dept.IT,Email="@gmail.com"},
                new Employee(){Id=3,Name="Sam",Department = Dept.IT,Email="sam@gmail.com"},
            };
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == Id);
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }

        public Employee Add(Employee emp)
        {
            emp.Id = _employeeList.Max(x => x.Id) + 1;
            _employeeList.Add(emp);
            return emp;
        }

        public Employee Update(Employee empChanges)
        {
            Employee employee = _employeeList.FirstOrDefault(x => x.Id == empChanges.Id);
            if(employee!=null){
                employee.Name = empChanges.Name;
                employee.Email = empChanges.Email;
                employee.Department = empChanges.Department;
            }
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = _employeeList.FirstOrDefault(x => x.Id == id);
            if(employee!=null){
                _employeeList.Remove(employee);
            }
            return employee;
        }
    }
}
