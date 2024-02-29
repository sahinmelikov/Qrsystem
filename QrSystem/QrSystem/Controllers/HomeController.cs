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
        [HttpGet]
        public IActionResult Index()
        {
            // Post işleminden sonra qrCodeId değerini saklamak için ViewBag kullanabiliriz
            // Bu sayede, post işlemi gerçekleştirilmediğinde ViewBag.QrCodeId değeri null olacaktır
            ViewBag.QrCodeId = null;

            HomeVM homeVM = new HomeVM()
            {
                Product = _appDbContext.Products.ToList(),
                QrCode = _appDbContext.QrCodes.ToList(),
                RestourantTables = _appDbContext.Tables.ToList(),
            };
            return View(homeVM);
        }

        [HttpPost]
        public IActionResult Index(int qrCodeId)
        {
            // Post işleminden sonra qrCodeId değerini saklayalım
            ViewBag.QrCodeId = qrCodeId;

            var masa = _appDbContext.Tables.FirstOrDefault(m => m.QrCodeId == qrCodeId);

            if (masa != null)
            {
                HomeVM homeVM = new HomeVM()
                {
                    Product = _appDbContext.Products.ToList(),
                    RestourantTables = _appDbContext.Tables.Where(d => d.QrCodeId == qrCodeId).ToList(),
                };
                return View(homeVM);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Masa bulunamadı. Lütfen geçerli bir QR kodu girin.");
                return View();
            }
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
