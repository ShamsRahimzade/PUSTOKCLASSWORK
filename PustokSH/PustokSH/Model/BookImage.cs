using Microsoft.AspNetCore.Identity;

namespace PustokSH.Model
{
    public class BookImage
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public bool? IsPrimary { get; set; }
        public int BookId { get; set; }
        public Book book { get; set; }
    }
}
