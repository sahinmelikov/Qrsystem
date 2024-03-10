namespace QrSystem.Models
{
    public class ParentsCategory
    {
        public int Id { get; set; } 
        public string Name { get; set; }
       
        public List<Product> Products { get; set; }
       
    }
}
