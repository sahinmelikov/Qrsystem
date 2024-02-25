using QrSystem.Models;

namespace QrSystem.ViewModel
{
    public class HomeVM
    {
        public List<QrCode> QrCode { get; set; }
        public List<Product> Product { get; set; }
        public List<RestourantTables> RestourantTables { get; set; }
    }
}
