using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeService.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace EmployeeService.Models
{
    public class EmployeeRepository : IEmployeeRepositiry
    {
        private string connectionString = null;
        public EmployeeRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<int> Create(Employee employee)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var sqlQuery = $@"INSERT INTO Employee(Name, Surname, Phone, CompanyId, Passport, Department) VALUES
                                          (@Name, @Surname, @Phone, @CompanyId,@Passport, @Department); SELECT CAST(SCOPE_IDENTITY() as int)";
                int employeeId = await connection.QueryFirstOrDefaultAsync<int>(sqlQuery, EmployeeDTO.ToDTO(employee)).ConfigureAwait(false);
                return employeeId;
            }
        }

        public async Task Delete(int id)
        {

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var sqlQuery = $@"DELETE FROM Employee WHERE Id = @id";
                await connection.ExecuteAsync(sqlQuery, new { id });
            }
        }

        public IEnumerable<Employee> GetEmployeesByCompany(int CompanyId)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {

                var sqlQuery = $@"SELECT Name, Surname, Phone, CompanyId, Passport, Department
                                         FROM Employee WHERE CompanyId = @CompanyId";
                
                EmployeeDTO[] employees = connection.QueryAsync<EmployeeDTO>(sqlQuery, new { CompanyId }).GetAwaiter().GetResult().ToArray();
                return Employee.FromDTO(employees);
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartment(string department)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var sqlQuery = $@"SELECT * FROM Employee WHERE Department = @Department";
                var query = await connection.QueryMultipleAsync(sqlQuery, new { department});
                var result = Employee.FromDTO(await query.ReadAsync<EmployeeDTO>());
                return result;
            }
        }

        public async Task Update(Employee employee)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var sqlQuery = $@"UPDATE Employee SET Name = @Name, Surname = @Surname, Phone = @Phone, CompanyId = @CompanyId, Passport = @Passport, Department = @Department WHERE Id = @Id";
                await connection.ExecuteAsync(sqlQuery, EmployeeDTO.ToDTO(employee));
            }
        }
    }
}
