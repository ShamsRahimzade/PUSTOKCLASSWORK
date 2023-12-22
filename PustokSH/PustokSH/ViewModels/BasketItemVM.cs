namespace PustokSH.ViewModels
{
    public class BasketItemVM
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public decimal Price { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
