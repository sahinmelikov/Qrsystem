namespace QrSystem.ViewModel
{
    public class UrunlerViewModel
    {
        public Dictionary<int, Dictionary<string, List<BasketİtemVM>>> UrunlerByQrCodeAndTable { get; set; }

        public UrunlerViewModel()
        {
            UrunlerByQrCodeAndTable = new Dictionary<int, Dictionary<string, List<BasketİtemVM>>>();
        }
    }


}
