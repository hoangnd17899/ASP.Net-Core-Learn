using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDBContext context;
        private readonly ILogger logger;

        public SQLEmployeeRepository(AppDBContext _context,ILogger<SQLEmployeeRepository> _logger)
        {
            logger=_logger;
            context = _context;
        }

        public Employee GetEmployee(int Id)
        {
            logger.LogTrace("Trace log");
            logger.LogDebug("Debug log");
            logger.LogInformation("Information log");
            logger.LogWarning("Warning log");
            logger.LogError("Error log");
            logger.LogCritical("Critial log");

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
