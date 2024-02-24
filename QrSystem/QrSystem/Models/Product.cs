namespace QrSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public QrCode QrCode { get; set;}
        public int QrCodeId { get; set; }

    }
}
