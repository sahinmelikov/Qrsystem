using QrSystem.Models;

namespace QrSystem.ViewModel
{
    public class TableVM
    {
        public int TableNumber { get; set; }
        public QrCode QrCode { get; set; }
        public int QrCodeId { get; set; }
        public List<Product> Products { get; set; }
    }
}
