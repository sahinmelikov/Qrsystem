namespace QrSystem.Models
{
    public class SaxlanilanSifarish
    {
        public int Id { get; set; }
        public int QrCodeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int ProductCount { get; set; }
        public string ImagePath { get; set; }
        public string TableName { get; set; }
        public bool IsApproved { get; set; } // Onay durumu
    }
}
