using HRMS.API.DTOs.Auth;

namespace HRMS.API.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task CreateUserAsync(CreateUserDto createUserDto);
    }
}
