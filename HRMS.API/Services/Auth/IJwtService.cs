using HRMS.API.DTOs.Auth;
using HRMS.API.Models;

namespace HRMS.API.Services.Auth
{
    public interface IJwtService
    {
        LoginResponseDto GenerateToken(User user);
    }
}
