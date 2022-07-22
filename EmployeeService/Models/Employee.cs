
using System.Collections.Generic;

namespace EmployeeService.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public int CompanyId { get; set; }
        public Passport Passport { get; set; }
        public Department Department { get; set; }

        public static Employee FromDTO(EmployeeDTO dto)
        {
            return new Employee
            {
                Id = dto.Id,
                Name = dto.Name,
                Surname = dto.Surname,
                Phone = dto.Phone,
                CompanyId = dto.CompanyId,
                Passport = Passport.FromString(dto.Passport),
                Department = Department.FromString(dto.Department)
            };
        }
        public static IEnumerable<Employee> FromDTO(IEnumerable<EmployeeDTO> dto)
        {
            List<Employee> result = new List<Employee>();
            foreach (var item in dto)
            {
                result.Add(new Employee
                {
                    Id = item.Id,
                    Name = item.Name,
                    Surname = item.Surname,
                    Phone = item.Phone,
                    CompanyId = item.CompanyId,
                    Passport = Passport.FromString(item.Passport),
                    Department = Department.FromString(item.Department)
                });
            }
            return result;
        }
    }
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public int CompanyId { get; set; }
        public string Passport { get; set; }
        public string Department { get; set; }
        public static EmployeeDTO ToDTO(Employee employee)
        {
            return new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Surname = employee.Surname,
                Phone = employee.Phone,
                CompanyId = employee.CompanyId,
                Passport = employee.Passport.ToString(),
                Department = employee.Department.ToString()
            };
        }
    }
}
