using System.ComponentModel.DataAnnotations.Schema;

namespace PustokSH.Areas.PustokAdmin.ViewModels
{
    public class UpdateSlideVM
    {
        

        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Desc { get; set; }
        public string? Img { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
