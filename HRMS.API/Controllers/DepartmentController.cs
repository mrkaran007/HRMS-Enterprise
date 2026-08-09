using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers
{
    public class DepartmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
