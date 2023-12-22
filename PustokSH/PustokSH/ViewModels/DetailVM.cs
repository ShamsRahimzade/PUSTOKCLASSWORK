using PustokSH.Model;

namespace PustokSH.ViewModels
{
    public class DetailVM
    {
        public Book  book { get; set; }
        public List<Book> relatedbooks { get; set; }
    }
}
