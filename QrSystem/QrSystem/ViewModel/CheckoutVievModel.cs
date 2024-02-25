using QrSystem.Models;

namespace QrSystem.ViewModel
{
    public class CheckoutViewModel
    {
        public List<BasketİtemVM> ProductsInCart { get; set; }
        public double TotalAmount { get; set; }
        public string TableName { get; set; } // Masa adı veya numarası
    }
}
