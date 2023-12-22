using System.ComponentModel.DataAnnotations;

namespace PustokSH.ViewModels
{
    public class LoginVM
    {
        [Required]
        [MinLength(3)]
        [MaxLength(320)]
        public string UserNameOrEmail { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemembered { get; set; }
    }
}
