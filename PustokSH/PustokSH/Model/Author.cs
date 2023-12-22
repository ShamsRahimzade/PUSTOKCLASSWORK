using System.ComponentModel.DataAnnotations;

namespace PustokSH.Model
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        public List<Book>? books { get; set; }
    }
}
