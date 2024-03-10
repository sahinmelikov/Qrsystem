using Microsoft.AspNetCore.Mvc;
using QrSystem.DAL;
using QrSystem.Models;
using QrSystem.ViewModel;

namespace QrSystem.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class HomeController : Controller
    {
        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;
        public HomeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            IFormFile file = productVM.ImageFile;
            if (!file.ContentType.Contains("image/"))
            {

                ModelState.AddModelError("Image", "Sizin gonderdiyiniz sekil deyil.");
                return View();
            }
            if (!(productVM.ImageFile.Length / 1024 / 1024 < 2))
            {
                ModelState.AddModelError("Image", "Sizin gonderdiyiniz sekilin olcusu 2mb-dan coxdur.");
                return View();
            }
            string filename = Guid.NewGuid() + file.FileName;
            using (FileStream stream = new FileStream(Path.Combine(_env.WebRootPath, "Image", "Product", filename), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Product product = new Product { Name = productVM.Name, Price = productVM.Price, Description = productVM.Description, ImagePath = filename, Quantity = productVM.Quantity,ParentsCategoryId=productVM.ParentsCategoryId };
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();
            Product product = _context.Products.Find(id);
            string rootPath = Path.Combine(_env.WebRootPath, "Image", "Product");
            string oldFilePath = Path.Combine(rootPath, product.ImagePath);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            if (product is null) return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
