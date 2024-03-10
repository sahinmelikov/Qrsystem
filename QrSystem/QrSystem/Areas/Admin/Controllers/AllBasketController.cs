using Microsoft.AspNetCore.Mvc;

namespace QrSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AllBasketController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
