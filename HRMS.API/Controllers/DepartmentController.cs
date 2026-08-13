using HRMS.API.DTOs.Department;
using HRMS.API.Services.Department;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        #region GetAllDepartments
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            
            var departments = await _departmentService.GetAllDepartmentsAsync();

            return Ok(departments);
        }
        #endregion

        #region GetDepartmentById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }
        #endregion

        #region CreateDepartment
        [HttpPost]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentDto departmentDto)
        {
            var department = await _departmentService.CreateDepartmentAsync(departmentDto);
            
            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.DepartmentId }, department);
        }
        #endregion

        #region UpdateDepartment
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto departmentDto)
        {
            var department = await _departmentService.UpdateDepartmentAsync(id, departmentDto);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }
        #endregion

        #region DeleteDepartment
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var status = await _departmentService.DeleteDepartmentAsync(id);
            if (!status)
            {
                return NotFound();
            }
            return NoContent();
        }
        #endregion
    }
}
