using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeService.Interfaces;
using EmployeeService.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace EmployeeService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            repositiry = new EmployeeRepository(configuration.GetConnectionString("DefaultConnection"));
        }
        private IEmployeeRepositiry repositiry;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Run(async (context) =>
            {
                var response = context.Response;
                var request = context.Request;
                var path = request.Path.Value;

                string expressionForNumbers = @"^/api/v1/Employee/\d+$";
                string expressionForDepartment = @"^/api/v1/Employee/.+,.+$";
                //Get api/v1/Employee/CompanyId
                if (Regex.IsMatch(path, expressionForNumbers) && request.Method == "GET")
                {
                    int? param = Convert.ToInt32(path.Split("/")[4]);
                    if (param != null)
                    {
                        await response.WriteAsync(JsonConvert.SerializeObject(repositiry.GetEmployeesByCompany((int)param)));
                        response.StatusCode = 200;
                    }
                    else
                        response.StatusCode = 402;
                }
                //Get api/v1/Employee/Department
                else if (Regex.IsMatch(path, expressionForDepartment) && request.Method == "GET")
                {
                    string param = path.Split('/')[4];
                    if (!string.IsNullOrEmpty(param))
                    {
                        string json = JsonConvert.SerializeObject(await repositiry.GetEmployeesByDepartment(param));
                        await response.WriteAsync(json);
                    }
                    else
                        response.StatusCode = 400;
                }
                else if (path == "/api/v1/Employee" && request.Method == "POST")
                {
                    await CreateEmployeeAsync(response, request);
                }
                else if (path == "/api/v1/Employee" && request.Method == "PUT")
                {
                    await UpdateEmployeeAsync(response, request);
                }
                //Get api/v1/Employee/EmployeeId
                else if (Regex.IsMatch(path, expressionForNumbers) && request.Method == "DELETE")
                {
                    int? param = Convert.ToInt32(path.Split('/')[4]);
                    if (param != null)
                    {
                        DeleteEmployee((int)param, response);
                    }
                    else
                        response.StatusCode = 400;
                }
            });
        }

        private void DeleteEmployee(int param, HttpResponse response)
        {
            try
            {
                repositiry.Delete(param);
            }
            catch
            {
                response.StatusCode = 404;
                response.WriteAsync("Employee not found");
            }
        }

        private async Task UpdateEmployeeAsync(HttpResponse response, HttpRequest request)
        {

                string json;
                using (StreamReader reader = new StreamReader(request.Body, Encoding.Default))
                {
                    json = await reader.ReadToEndAsync();
                }
                var employee = JsonConvert.DeserializeObject<Employee>(json);
                if (employee != null)
                {
                    await repositiry.Update(employee);
                }
                else
                {
                    throw new Exception("Wrong data");
                }
            

        }

        private async Task CreateEmployeeAsync(HttpResponse response, HttpRequest request)
        {
            try
            {
                string json;
                int result;
                using (StreamReader reader = new StreamReader(request.Body, Encoding.Default))
                {
                    json = await reader.ReadToEndAsync();
                }
                var employee = JsonConvert.DeserializeObject<Employee>(json);
                if (employee != null)
                {
                    result = await repositiry.Create(employee);
                    await response.WriteAsync(JsonConvert.SerializeObject(result));
                }
                else
                {
                    throw new Exception("Wrong data");
                }
            }
            catch
            {
                response.StatusCode = 400;
                await response.WriteAsync(JsonConvert.SerializeObject("Wrong data"));
            }
        }
    }
}
