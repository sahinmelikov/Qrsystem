using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QrSystem.DAL;
using QrSystem.Models;
using QrSystem.ViewModel;

namespace QrSystem.Controllers
{
    public class BasketController : Controller
    {

        private const string COOKIES_BASKET = "basketVM";
        private readonly AppDbContext _appDbContext;

        public BasketController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        private void SetBasketItemCountInViewBag()
        {
            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");
            int itemCount = basketVMs.Sum(b => b.Count);
            ViewBag.BasketItemCount = itemCount;
        }
        [HttpGet]
        public IActionResult Index(int? qrCodeId)
        {
            if (!qrCodeId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "QR kodu belirtilmedi.");
                return View(new List<BasketİtemVM>());
            }

            var masa = _appDbContext.Tables
                                     .Include(t => t.Products)
                                     .FirstOrDefault(m => m.QrCodeId == qrCodeId);
            if (masa == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz QR kodu. Lütfen doğru bir QR kodu girin.");
                return View(new List<BasketİtemVM>());
            }
            ViewBag.QrCodeId = qrCodeId;
            var basketCookieName = COOKIES_BASKET + "_" + qrCodeId;
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[basketCookieName] ?? "[]");

            List<BasketİtemVM> basketItemVMs = new List<BasketİtemVM>();

            foreach (var basketItem in basketVMList)
            {
                var product = _appDbContext.Products.FirstOrDefault(p => p.Id == basketItem.ProductId);
                if (product != null)
                {
                    basketItemVMs.Add(new BasketİtemVM
                    {
                        Name = product.Name,
                        Id = product.Id,
                        Price=product.Price,
                        ImagePath=product.ImagePath,
                        
                        QrCodeId = qrCodeId.Value,
                        ProductId = basketItem.ProductId,
                        ProductCount = basketItem.Count // Ürün sayısını burada ekleyin
                    });
                }
            }

            return View(basketItemVMs);
        }
      

        [HttpPost]
        public IActionResult AddBasket(int qrCodeId, int productId)
        {
            var masa = _appDbContext.Tables.FirstOrDefault(m => m.QrCodeId == qrCodeId);
            if (masa == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz QR kodu. Lütfen doğru bir QR kodu girin.");
                return RedirectToAction("Index");
            }

            var product = _appDbContext.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Ürün bulunamadı. Lütfen geçerli bir ürün seçin.");
                return RedirectToAction("Index", new { qrCodeId });
            }

            var basketCookieName = COOKIES_BASKET + "_" + qrCodeId; // Her QR kodu için farklı bir cookie adı oluştur
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[basketCookieName] ?? "[]");

            var basketItem = basketVMList.FirstOrDefault(b => b.ProductId == productId);
            if (basketItem != null)
            {
                basketItem.Count++;
            }
            else
            {
                basketVMList.Add(new BasketVM { ProductId = productId, Count = 1 });
            }

            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Append(basketCookieName, JsonConvert.SerializeObject(basketVMList), cookieOptions);

            return RedirectToAction("Index","Home", new { qrCodeId });
        }


  
        public IActionResult RemoveFromBasket(int id)
        {
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");

            BasketVM cookiesBasket = basketVMList.FirstOrDefault(s => s.ProductId == id);
            if (cookiesBasket != null)
            {
                if (cookiesBasket.Count > 1)
                {
                    cookiesBasket.Count--;
                }
                else
                {
                    basketVMList.Remove(cookiesBasket);
                }
            }

            Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));

            SetBasketItemCountInViewBag();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveItemFromBasket(int qrCodeId, int id)
        {
            string basketCookieName = COOKIES_BASKET + "_" + qrCodeId;
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[basketCookieName] ?? "[]");

            BasketVM productToRemove = basketVMList.FirstOrDefault(s => s.ProductId == id);
            if (productToRemove != null)
            {
                basketVMList.Remove(productToRemove);

                Response.Cookies.Append(basketCookieName, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));
            }

            SetBasketItemCountInViewBag();

            return RedirectToAction("Index", new { qrCodeId });
        }


        [HttpPost]
        public IActionResult UpdateBasketItem(int qrCodeId, int productId, string action)
        {
            var basketCookieName = COOKIES_BASKET + "_" + qrCodeId;
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[basketCookieName] ?? "[]");

            BasketVM basketItem = basketVMList.FirstOrDefault(s => s.ProductId == productId);
            if (basketItem != null)
            {
                if (action == "increment")
                {
                    basketItem.Count++;

                    // Ürün miktarını azalt
                    Product product = _appDbContext.Products.FirstOrDefault(p => p.Id == productId);
                    if (product != null)
                    {
                        product.Quantity--;
                        _appDbContext.SaveChanges();
                    }
                }
                else if (action == "decrement")
                {
                    if (basketItem.Count > 1)
                    {
                        basketItem.Count--;

                        // Ürün miktarını azalt
                        Product product = _appDbContext.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            product.Quantity++; // Ürün miktarını artır
                            _appDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        basketItem.Count = 1; // Ürün miktarını sadece 1 tane yap
                    }
                }


            }

            Response.Cookies.Append(basketCookieName, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));

            SetBasketItemCountInViewBag();

            return RedirectToAction("Index", new { qrCodeId });
        }

        public IActionResult RemoveAllItemsFromBasket(int qrCodeId)
        {
            string basketCookieName = COOKIES_BASKET + "_" + qrCodeId;

            // Sepet öğelerini temizle
            Response.Cookies.Delete(basketCookieName);

            return RedirectToAction("Index", new { qrCodeId });
        }
        [HttpPost]
        public IActionResult Checkout(int qrCodeId)
        {
            // Belirli bir QR koduna sahip sepet ürünlerini al
            List<BasketİtemVM> productsInCart = GetProductsInCart(qrCodeId);

            // Sepetteki ürünleri onayla
            SaveApprovedProductsToSession(qrCodeId, productsInCart);

            // urun.cshtml sayfasına ViewModel'i gönder
            return RedirectToAction("Urun");
        }

        public IActionResult Urun()
        {
            var viewModel = new UrunlerViewModel();

            foreach (var key in HttpContext.Session.Keys)
            {
                if (key.StartsWith("ApprovedProducts-"))
                {
                    var qrCodeId = int.Parse(key.Split('-')[1]);
                    var productsJson = HttpContext.Session.GetString(key);
                    var approvedProducts = JsonConvert.DeserializeObject<List<BasketİtemVM>>(productsJson);

                    // QR kodu ve ona ait ürünleri ViewModel'e ekleyin
                    if (!viewModel.UrunlerByQrCodeAndTable.ContainsKey(qrCodeId))
                    {
                        viewModel.UrunlerByQrCodeAndTable[qrCodeId] = new Dictionary<string, List<BasketİtemVM>>();
                    }

                    foreach (var product in approvedProducts)
                    {
                        if (!viewModel.UrunlerByQrCodeAndTable[qrCodeId].ContainsKey(product.TableName))
                        {
                            viewModel.UrunlerByQrCodeAndTable[qrCodeId][product.TableName] = new List<BasketİtemVM>();
                        }
                        viewModel.UrunlerByQrCodeAndTable[qrCodeId][product.TableName].Add(product);
                    }
                }
            }

            return View(viewModel); // Burada viewModel nesnesini görünüme geçirin
        }

        private void SaveApprovedProductsToSession(int qrCodeId, List<BasketİtemVM> approvedProducts)
        {
            var sessionKey = $"ApprovedProducts-{qrCodeId}";
            HttpContext.Session.SetString(sessionKey, JsonConvert.SerializeObject(approvedProducts));
        }

        private List<BasketİtemVM> GetProductsInCart(int qrCodeId)
        {
            List<BasketİtemVM> basketItemVMs = new List<BasketİtemVM>();

            var basketCookieName = COOKIES_BASKET + "_" + qrCodeId; // Her QR kodu için farklı bir cookie adı oluştur
            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[basketCookieName] ?? "[]");

            foreach (var item in basketVMs)
            {
                Product product = _appDbContext.Products
                    .Include(p => p.Tables)
                    .FirstOrDefault(p => p.Id == item.ProductId);

                if (product != null)
                {
                    var tableName = item.TableNumber.ToString();
                    var existingItem = basketItemVMs.FirstOrDefault(b => b.ProductId == product.Id && b.TableName == tableName);
                    if (existingItem != null)
                    {
                        existingItem.ProductCount += item.Count; // Ürün zaten varsa miktarını artır
                    }
                    else
                    {
                        basketItemVMs.Add(new BasketİtemVM
                        {
                            Name = product.Name,
                            ProductId = product.Id,
                            Description = product.Description,
                            Price = product.Price,
                            ProductCount = item.Count,
                            ImagePath = product.ImagePath,
                            TableName = tableName
                        });
                    }
                }
            }

            return basketItemVMs;
        }

    }

}

