using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace imagestore.Controllers
{
    public class ManageController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
