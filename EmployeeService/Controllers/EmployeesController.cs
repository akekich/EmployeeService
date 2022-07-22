using EmployeeService.Interfaces;
using EmployeeService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmployeeService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepositiry _repository;
        public EmployeesController(IEmployeeRepositiry repository)
        {
            _repository = repository;
        }
        [HttpGet("{param}")]
        public async Task<string> GetEmployees(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }
            string expressionForNumbers = @"\d+$";
            string expressionForDepartment = @".+,.+$";
            if (Regex.IsMatch(param, expressionForDepartment))
            {
                return await GetByDepartment(param);
            }

            else if (Regex.IsMatch(param, expressionForNumbers))
            {
                return GetByCompanyId(Convert.ToInt32(param));
            }
            else
            {
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }
        }
        public string GetByCompanyId(int id)
        {
            try
            {
                Employee[] employees = _repository.GetEmployeesByCompany(id).ToArray();
                if (employees.Length != 0)
                {
                    return JsonConvert.SerializeObject(employees);
                }
                else
                {
                    Response.StatusCode = 404;
                    return JsonConvert.SerializeObject(new { message = "Not found" });
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }
        }

        public async Task<string> GetByDepartment(string department)
        {
            if (!string.IsNullOrWhiteSpace(department))
            {
                var employees = await _repository.GetEmployeesByDepartment(department);
                if (employees.Count() != 0)
                {
                    return JsonConvert.SerializeObject(employees);
                }
                else
                {
                    Response.StatusCode = 404;
                    return JsonConvert.SerializeObject(new { message = "Not found" });
                }
            }
            else
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }
        }

        [HttpPost]
        public async Task<string> CreateEmployee()
        {
            var requestBody = Request.Body;
            string json;
            if (requestBody == null)
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }

            using (var reader = new StreamReader(requestBody, Encoding.Default))
            {
                json = await reader.ReadToEndAsync();
            }
            try
            {
                Employee employee = JsonConvert.DeserializeObject<Employee>(json);
                if (employee != null)
                {
                    int employeeId = await _repository.Create(employee);
                    return JsonConvert.SerializeObject(new { employeeId });
                }
                else
                {
                    Response.StatusCode = 404;
                    return JsonConvert.SerializeObject(new { message = "Not found" });
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }
        }

        [HttpPut]
        public async Task<string> UpdateEmployee()
        {
            var requestBody = Request.Body;
            string json;
            if (requestBody == null)
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }

            using (var reader = new StreamReader(requestBody, Encoding.Default))
            {
                json = await reader.ReadToEndAsync();
            }
            try
            {
                Employee employee = JsonConvert.DeserializeObject<Employee>(json);
                if (employee != null)
                {
                    await _repository.Update(employee);
                    return JsonConvert.SerializeObject("OK");
                }
                else
                {
                    Response.StatusCode = 404;
                    return JsonConvert.SerializeObject(new { message = "Not found" });
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<string> DeleteEmployee(int id)
        {
            try
            {
                await _repository.Delete(id);
                return JsonConvert.SerializeObject("OK");
            }
            catch
            {
                Response.StatusCode = 400;
                return JsonConvert.SerializeObject(new { message = "Wrong input" });
            }
        }
    }
}
