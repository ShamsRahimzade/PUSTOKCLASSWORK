using System.ComponentModel.DataAnnotations.Schema;

namespace PustokSH.Model
{
    public class Slide
    {


        public int Id { get; set; }

        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Desc { get; set; }
        public string? Img { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
