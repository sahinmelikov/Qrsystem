namespace QrSystem.Models
{
    public class RestourantTables
    {
        public int Id { get; set; } 
        public int TableNumber { get; set; }
        public QrCode QrCode { get; set; }
        public int QrCodeId { get; set; }
        public List<Product> Products { get; set; }

    }
}
