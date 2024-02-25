using Microsoft.AspNetCore.Mvc;
using QrSystem.DAL;
using QrSystem.Models;
using QrSystem.ViewModel;
using System.Diagnostics;

namespace QrSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Product = _appDbContext.Products.ToList(),
            };
            return View(homeVM);
        }
        public IActionResult Sebet()
        {

            HomeVM homeVM = new HomeVM()
            {
                Product = _appDbContext.Products.ToList(),
            };
            return View(homeVM);
        }
     

    }
}
