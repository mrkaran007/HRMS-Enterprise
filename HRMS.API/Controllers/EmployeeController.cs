using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
