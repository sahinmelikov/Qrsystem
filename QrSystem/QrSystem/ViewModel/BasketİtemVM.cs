using System.ComponentModel.DataAnnotations;

namespace QrSystem.ViewModel
{
    public class BasketİtemVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        public string TableName { get; set; }
        public int QrCodeId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public int ProductCount { get; set; }
    }
}
