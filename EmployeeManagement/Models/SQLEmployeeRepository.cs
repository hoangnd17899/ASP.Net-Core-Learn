using System;
using System.Linq;
using System.Collections.Generic;

namespace EmployeeManagement
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDBContext context;

        public SQLEmployeeRepository(AppDBContext _context)
        {
            context = _context;
        }

        public Employee GetEmployee(int Id)
        {
            return context.Employees.FirstOrDefault(e => e.Id == Id);
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return context.Employees.ToList();
        }

        public Employee Add(Employee emp)
        {
            emp.Id = context.Employees.Max(x => x.Id) + 1;
            context.Employees.Add(emp);
            context.SaveChanges();
            return emp;
        }

        public Employee Update(Employee empChanges)
        {
            var employee = context.Employees.Attach(empChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return empChanges;
        }

        public Employee Delete(int id)
        {
            Employee employee = context.Employees.FirstOrDefault(x => x.Id == id);
            if(employee!=null){
                context.Employees.Remove(employee);
                context.SaveChanges();
            }
            return employee;
        }
    }
}
