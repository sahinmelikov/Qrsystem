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
        public IActionResult Index(int? qrCodeId ,int productId)
        {
            if (!qrCodeId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "QR kodu belirtilmedi.");
                return View(new List<BasketİtemVM>());
            }

            // Masa nesnesini alırken, QR kodunu ve ürün ID'sini belirtin
            var masa = _appDbContext.Tables.FirstOrDefault(m => m.QrCodeId == qrCodeId.Value && m.Products.Any(p => p.Id == productId));

            if (masa == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz QR kodu. Lütfen doğru bir QR kodu girin.");
                return View(new List<BasketİtemVM>());
            }

            if (masa.Products == null || !masa.Products.Any())
            {
                ModelState.AddModelError(string.Empty, "Bu masaya henüz ürün eklenmemiş.");
                return View(new List<BasketİtemVM>());
            }

            List<BasketİtemVM> basketItemVMs = masa.Products.Select(product => new BasketİtemVM
            {
                Name = product.Name,
                Id = product.Id,
                Description = product.Description,
                Price = product.Price,
                ProductCount = 1, // Varsayılan olarak her üründen bir tane ekleyebilirsiniz.
                ImagePath = product.ImagePath,
                QrCodeId = qrCodeId.Value, // QR kodunu sakla
                ProductId = product.Id // Ürün ID'sini sakla
            }).ToList();

            SetBasketItemCountInViewBag();

            return View(basketItemVMs);
        }



        [HttpPost]
        public IActionResult AddBasket(int qrCodeId, int productId)
        {
            // QR kodunu kullanarak ilgili masayı bul
            var masa = _appDbContext.Tables.FirstOrDefault(m => m.QrCodeId == qrCodeId);
            ViewBag.QrCodeId = qrCodeId;
            ViewBag.ProductId = productId;
            if (masa == null)
            {
                // Geçersiz QR kodu hatası ekle
                ModelState.AddModelError(string.Empty, "Geçersiz QR kodu. Lütfen doğru bir QR kodu girin.");
                return RedirectToAction("Index");
            }

            // Masada belirtilen ürünü bul
            var product = _appDbContext.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                // Ürün bulunamadı hatası ekle
                ModelState.AddModelError(string.Empty, "Ürün bulunamadı. Lütfen geçerli bir ürün seçin.");
                return RedirectToAction("Index", new { qrCodeId });
            }

            // Sepet bilgisini cookie'den al
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");

            // Masaya belirtilen ürünü ekle
            BasketVM cookiesBasket = basketVMList.FirstOrDefault(s => s.ProductId == productId);
            if (cookiesBasket != null)
            {
                cookiesBasket.Count++;
            }
            else
            {
                BasketVM basketVM = new BasketVM() { ProductId = productId, Count = 1 };
                basketVMList.Add(basketVM);
            }

            // Güncellenmiş sepet bilgisini cookie içerisine yaz
            Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));

            // Sepete ekledikten sonra, kullanıcıyı tekrar ilgili QR koduna sahip masa sayfasına yönlendir
            return RedirectToAction("Index", new { qrCodeId });
        }

        // Sepete ürün eklemek için yardımcı bir metot
        private void AddProductToBasket(int productId)
        {
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");

            BasketVM cookiesBasket = basketVMList.FirstOrDefault(s => s.ProductId == productId);
            if (cookiesBasket != null)
            {
                cookiesBasket.Count++;
            }
            else
            {
                BasketVM basketVM = new BasketVM() { ProductId = productId, Count = 1 };
                basketVMList.Add(basketVM);
            }

            Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));

            SetBasketItemCountInViewBag();
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

        public IActionResult RemoveItemFromBasket(int id)
        {
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");

            BasketVM productToRemove = basketVMList.FirstOrDefault(s => s.ProductId == id);
            if (productToRemove != null)
            {
                basketVMList.Remove(productToRemove);

                Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));
            }

            SetBasketItemCountInViewBag();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateBasketItem(int productId, string action)
        {
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");

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

                        // Ürün miktarını artır
                        Product product = _appDbContext.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            product.Quantity++;
                            _appDbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        basketVMList.Remove(basketItem);

                        // Ürünü geri ekle
                        Product product = _appDbContext.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            product.Quantity++;
                            _appDbContext.SaveChanges();
                        }
                    }
                }
            }

            Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));

            SetBasketItemCountInViewBag();

            return RedirectToAction("Index", "Basket");
        }


        public IActionResult RemoveAllItemsFromBasket()
        {
            // Sepet öğelerini temizle
            Response.Cookies.Delete(COOKIES_BASKET);

            return RedirectToAction("Index");
        }
        public IActionResult Checkout()
        {
            // Sepet içeriğini al
            List<BasketİtemVM> productsInCart = GetProductsInCart();

            // ViewModel oluştur
            var viewModel = new Dictionary<string, List<BasketİtemVM>>();

            // Bir masa numarası belirterek ürünleri grupla
            foreach (var product in productsInCart)
            {
                if (product.TableName != null) // Check if TableName is not null
                {
                    if (!viewModel.ContainsKey(product.TableName))
                    {
                        viewModel[product.TableName] = new List<BasketİtemVM>();
                    }
                    viewModel[product.TableName].Add(product);
                }
            }

            // urun.cshtml sayfasına ViewModel'i gönder
            return View("~/Views/Basket/urun.cshtml", viewModel);
        }


        // Sepetteki ürünleri almak için örnek bir metot
        private List<BasketİtemVM> GetProductsInCart()
        {
            List<BasketİtemVM> basketItemVMs = new List<BasketİtemVM>();
            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");

            foreach (BasketVM item in basketVMs)
            {
                Product product = _appDbContext.Products
                    .Include(p => p.Tables)
                    .FirstOrDefault(p => p.Id == item.ProductId);

                if (product != null && product.Tables != null && product.Tables.Any())
                {
                    foreach (var table in product.Tables)
                    {
                        basketItemVMs.Add(new BasketİtemVM
                        {
                            Name = product.Name,
                            Id = product.Id,
                            Description = product.Description,
                            Price = product.Price,
                            ProductCount = item.Count,
                            ImagePath = product.ImagePath,
                            TableName = table.TableNumber.ToString()
                        });
                    }
                }
            }

            return basketItemVMs;
        }



    }

}

