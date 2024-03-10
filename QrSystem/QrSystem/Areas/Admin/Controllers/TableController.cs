using Microsoft.AspNetCore.Mvc;
using QrSystem.DAL;
using QrSystem.Models;
using QrSystem.ViewModel;

namespace QrSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TableController:Controller
    {
        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;
        public TableController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Tables.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TableVM tableVM)
        {
            RestourantTables tables = new RestourantTables { QrCodeId = tableVM.QrCodeId, TableNumber = tableVM.TableNumber };
            _context.Tables.Add(tables);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            RestourantTables tables= _context.Tables.Find(id);
            if (tables is null) return NotFound();
            _context.Tables.Remove(tables);

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
