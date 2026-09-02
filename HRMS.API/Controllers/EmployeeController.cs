using HRMS.API.DTOs.Employee;
using HRMS.API.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAuthorizationService _authorizationService;

        public EmployeeController(IEmployeeService employeeService, IAuthorizationService authorizationService)
        {
            _employeeService = employeeService;
            _authorizationService = authorizationService;
        }

        #region GetEmployees
        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] EmployeeSearchDto searchDto)
        {
            var employees = await _employeeService.GetEmployeesAsync(searchDto);
            return Ok(employees);
        }
        #endregion

        #region GetEmployeeById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            // Pass the employee ID as the resource for resource-based authorization.
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, "EmployeeAccess");

            // EmployeeAccess policy allows Admin/HR to access any employee,
            // while an Employee can access only their own record.
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }


            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            return Ok(employee);
        }
        #endregion

        #region GetCurrentUser
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity?.Name;
            var role = User.FindFirstValue(ClaimTypes.Role);
            var employeeId = User.FindFirstValue("EmployeeId");

            return Ok(new
            {
                UserId = userId,
                UserName = userName,
                Role = role,
                EmployeeId = employeeId
            });
        }
        #endregion

        #region CreateEmployee
        [Authorize(Roles = "admin,hr")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto employeeDto)
        {
            var employee = await _employeeService.CreateEmployeeAsync(employeeDto);
     
            return CreatedAtAction(
                nameof(GetEmployeeById), 
                new { id = employee.EmployeeId }, 
                employee);
        }
        #endregion

        #region UpdateEmployee
        [Authorize(Roles = "admin,hr")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto employeeDto)
        {
            var employee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);

            return Ok(employee);
        }
        #endregion

        #region DeleteEmployee
        [Authorize(Roles = "admin")]
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

        #region TransferEmployee
        [Authorize(Roles = "admin,hr")]
        [HttpPut("{id}/transfer")]
        public async Task<IActionResult> TransferEmployee(int id, TransferEmployeeRequestDto transferDto)
        {
            var result = await _employeeService.TransferEmployeeAsync(id, transferDto);
            return Ok(result);
        }
        #endregion

        #region GetEmployeeTransferHistory
        [HttpGet("{id}/transfer-history")]
        public async Task<IActionResult> GetEmployeeTransferHistory(int id)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, "EmployeeAccess");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var history = await _employeeService.GetEmployeeTransferHistoryAsync(id);
            return Ok(history);
        }
        #endregion

    }
}
