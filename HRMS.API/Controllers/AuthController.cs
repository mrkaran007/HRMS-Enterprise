using HRMS.API.DTOs.Auth;
using HRMS.API.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region CreateUser
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            await _authService.CreateUserAsync(createUserDto);

            return StatusCode(StatusCodes.Status201Created);
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var result = await _authService.LoginAsync(loginRequestDto);
            return Ok(result);
        }
        #endregion
    }
}
