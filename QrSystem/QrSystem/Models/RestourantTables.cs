namespace QrSystem.Models
{
    public class RestourantTables
    {
        public int Id { get; set; } 
        public int TableNumber { get; set; }
        public QrCode QrCode { get; set; }

    }
}
