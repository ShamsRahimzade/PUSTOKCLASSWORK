namespace PustokSH.Model
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Page { get; set; }
        public bool IsAvailable { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Discount { get; set; }
        public string Desc { get; set; }
        public int AuthorId { get; set; }
        public Author author { get; set; }
        public int GenreId { get; set; }
        public Genre genre { get; set; }

        public bool IsDeleted { get; set; } = false;
        public List<BookImage> bookImages { get; set; }

        public List<BookTag> bookTags { get; set; }

    }
}
