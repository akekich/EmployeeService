using EmployeeService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeService.Interfaces
{
    public interface IEmployeeRepositiry
    {
        public Task<int> Create(Employee employee);
        public Task Delete(int id);
        public IEnumerable<Employee> GetEmployeesByCompany(int companyId);
        public Task<IEnumerable<Employee>> GetEmployeesByDepartment(string department);
        public Task Update(Employee Employee);
    }
}
