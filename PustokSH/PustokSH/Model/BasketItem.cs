namespace PustokSH.Model
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public int BookId { get; set; }
        public Book book { get; set; }
        public string AppUserId { get; set; }
        public AppUser appUser { get; set; }
       
    }
}
