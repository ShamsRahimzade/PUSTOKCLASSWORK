using System.ComponentModel.DataAnnotations;

namespace PustokSH.Model
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public List<BookTag>? bookTags { get; set; }
    }
}
