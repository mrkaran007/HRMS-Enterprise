using HRMS.API.DTOs.Employee;
using HRMS.API.Models;
using HRMS.API.Services.Employee;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        #region GetAllEmployees
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            List<Employee> employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }
        #endregion

        #region GetEmployeeById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            Employee? employee = await _employeeService.GetEmployeeByIdAsync(id);
           
            return Ok(employee);
        }
        #endregion

        #region CreateEmployee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto employeeDto)
        {
            var employee = await _employeeService.CreateEmployeeAsync(employeeDto);
     
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.EmployeeId }, employee);
        }
        #endregion

        #region UpdateEmployee
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto employeeDto)
        {
            var employee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);

            return Ok(employee);
        }
        #endregion

        #region DeleteEmployee
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var status = await _employeeService.DeleteEmployeeAsync(id);
            if (!status)
            {
                return NotFound();
            }

            return NoContent();
        }
        #endregion

        #region SearchEmployeesByName
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployeesByNameAsync(string name)
        {
            var employees = await _employeeService.SearchEmployeesByNameAsync(name);
            return Ok(employees);
        }
        #endregion

        #region GetEmployeesWithMinimumSalary
        [HttpGet("salary")]
        public async Task<IActionResult> GetEmployeesWithMinimumSalaryAsync(decimal minimumSalary)
        {
            var employees = await _employeeService.GetEmployeesWithMinimumSalaryAsync(minimumSalary);
            return Ok(employees);
        }
        #endregion

    }
}
