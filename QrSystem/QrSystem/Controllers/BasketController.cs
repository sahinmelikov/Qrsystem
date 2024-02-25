using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            List<BasketİtemVM> basketItemVMs = new List<BasketİtemVM>();
            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");
            foreach (BasketVM item in basketVMs)
            {
                BasketİtemVM basketItemVM = _appDbContext.Products
                    .Where(s => s.Id == item.ProductId)
                    .Select(s => new BasketİtemVM
                    {
                        Name = s.Name,
                        Id = s.Id,
                        Description = s.Description,
                        Price = s.Price,
                      
                        ProductCount = item.Count,
                        ImagePath = s.ImagePath
                    })
                    .FirstOrDefault();
                if (basketItemVM != null)
                    basketItemVMs.Add(basketItemVM);
            }

            SetBasketItemCountInViewBag();

            return View(basketItemVMs);
        }

        public IActionResult AddBasket(int id)
        {
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");

            BasketVM cookiesBasket = basketVMList.FirstOrDefault(s => s.ProductId == id);
            if (cookiesBasket != null)
            {
                cookiesBasket.Count++;
            }
            else
            {
                BasketVM basketVM = new BasketVM() { ProductId = id, Count = 1 };
                basketVMList.Add(basketVM);
            }

            Response.Cookies.Append(COOKIES_BASKET, JsonConvert.SerializeObject(basketVMList.OrderBy(s => s.ProductId)));

            SetBasketItemCountInViewBag();

            return RedirectToAction("Index", "Home");
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
        public ActionResult Checkout()
        {
            // Sepet içeriğini almak için uygun bir şekilde değiştirin
            List<BasketİtemVM> productsInCart = GetProductsInCart(); // Sepet içeriği alınıyor

            // ViewModel oluştur
            var viewModel = new CheckoutViewModel
            {
                ProductsInCart = productsInCart,
                // Gerekirse başka veriler de buraya eklenir
            };

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
                BasketİtemVM basketItemVM = _appDbContext.Products
                    .Where(s => s.Id == item.ProductId)
                    .Select(s => new BasketİtemVM
                    {
                        Name = s.Name,
                        Id = s.Id,
                        Description = s.Description,
                        Price = s.Price,
                        ProductCount = item.Count,
                        ImagePath = s.ImagePath
                    })
                    .FirstOrDefault();
                if (basketItemVM != null)
                    basketItemVMs.Add(basketItemVM);
            }
            return basketItemVMs;
        }


        // Sepetteki belirli bir ürünün sayısını almak için örnek bir metot
        private int GetProductCountInCart(int productId)
        {
            List<BasketVM> basketVMList = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies[COOKIES_BASKET] ?? "[]");
            BasketVM productInCart = basketVMList.FirstOrDefault(p => p.ProductId == productId);
            return productInCart != null ? productInCart.Count : 0;
        }

    }

}

