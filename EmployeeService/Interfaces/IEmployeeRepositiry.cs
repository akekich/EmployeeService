using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeService.Models;

namespace EmployeeService.Interfaces
{
    interface IEmployeeRepositiry
    {
        public Task<int> Create(Employee employee);
        public Task Delete(int id);
        public IEnumerable<Employee> GetEmployeesByCompany(int companyId);
        public Task<IEnumerable<Employee>> GetEmployeesByDepartment(string department);
        public Task Update(Employee Employee);
    }
}
