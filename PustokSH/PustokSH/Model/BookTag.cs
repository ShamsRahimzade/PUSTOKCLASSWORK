namespace PustokSH.Model
{
    public class BookTag
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int TagId { get; set; }
        public Tag tag { get; set; }
    }
}
