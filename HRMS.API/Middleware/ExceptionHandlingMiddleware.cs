using HRMS.API.Exceptions;
using System.Net;
using System.Text.Json;

namespace HRMS.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (ex)
            {
                case BadRequestException:
                    //statusCode = (int)HttpStatusCode.BadRequest;
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case NotFoundException:
                    //statusCode = (int)HttpStatusCode.NotFound;
                    statusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    break;
                case ConflictException:
                    //statusCode = (int)HttpStatusCode.Conflict;
                    statusCode = StatusCodes.Status409Conflict;
                    message = ex.Message;
                    break;
                case UnauthorizedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = ex.Message;
                    break;
                default:
                    //statusCode = (int)HttpStatusCode.InternalServerError;
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                statusCode,
                message
            };
            //await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
