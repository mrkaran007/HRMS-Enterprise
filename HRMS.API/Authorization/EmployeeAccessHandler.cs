using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HRMS.API.Authorization
{
    public class EmployeeAccessHandler : AuthorizationHandler<EmployeeAccessRequirement, int>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            EmployeeAccessRequirement requirement, 
            int requestedEmployeeId)
        {
            var role = context.User.FindFirstValue(ClaimTypes.Role);
            var employeeIdClaim = context.User.FindFirstValue("EmployeeId");

            // Admin and HR can access any employee
            if(role == "admin" || role == "hr")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Employees can access only their own employee resource.
            if (!int.TryParse(employeeIdClaim, out var currentEmployeeId))
            {
                return Task.CompletedTask;
            }

            if(currentEmployeeId == requestedEmployeeId)
            {
                context.Succeed(requirement);
            }
            

            return Task.CompletedTask;
        }
    }
}
