using HRMS.API.DTOs.Auth;
using HRMS.API.Exceptions;
using HRMS.API.Models;
using HRMS.API.Repositories;
using HRMS.API.Repositories.Employee;
using HRMS.API.Repositories.User;
using Microsoft.AspNetCore.Identity;

namespace HRMS.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        #region CreateUserAsync
        public async Task CreateUserAsync(CreateUserDto createUserDto)
        {
            var userName = createUserDto.UserName.Trim();
            var userNameExists = await _userRepository.UserNameExistsAsync(userName);
            if (userNameExists)
            {
                throw new ConflictException("Username already exists.");
            }

            var email = createUserDto.Email.ToLower().Trim();
            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
            {
                throw new ConflictException("Email already exists.");
            }

            var role = createUserDto.Role.ToLower().Trim();
            if (!(role == "admin" || role == "hr" || role == "employee") )
            {
                throw new BadRequestException("Invalid role.");
            }

            if (role == "admin" && createUserDto.EmployeeId.HasValue)
            {
                throw new BadRequestException(
                    "Admin user cannot be linked to an employee.");
            }

            if ((role == "hr" || role == "employee") &&
                !createUserDto.EmployeeId.HasValue)
            {
                throw new BadRequestException(
                    $"{role} user must be linked to an employee.");
            }

            if (createUserDto.EmployeeId.HasValue)
            {
                var employeeExists = await _employeeRepository.ExistsAsync(createUserDto.EmployeeId.Value);

                if (!employeeExists)
                {
                    throw new NotFoundException("Employee not found.");
                }

                var employeeAlreadyHasUserAccount = await _userRepository.EmployeeHasUserAccountAsync(createUserDto.EmployeeId.Value);

                if (employeeAlreadyHasUserAccount)
                {
                    throw new ConflictException("Employee already has a user account.");
                }
            }

            //Password hashing will come here
            var user = new User
            {
                UserName = userName,
                Email = email,
                Role = role,
                IsActive = true,
                EmployeeId = createUserDto.EmployeeId
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, createUserDto.Password);

            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();

        }
        #endregion

        #region LoginAsync
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userRepository.GetByUserNameAsync(loginRequestDto.UserName.Trim());
            if (user == null)
            {
                throw new UnauthorizedException("Invalid username or password.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException("User account is inactive.");
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequestDto.Password);
            if (passwordResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Invalid username or password.");
            }

            // JWT generation will come here
            

            return _jwtService.GenerateToken(user);
        }
        #endregion


    }
}
