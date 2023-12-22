using System.ComponentModel.DataAnnotations.Schema;

namespace PustokSH.Areas.PustokAdmin.ViewModels
{
    public class CreateSlideVM
    {
      

        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Desc { get; set; }
      
        public int Order { get; set; }
      
        public IFormFile Photo { get; set; }
    }
}
