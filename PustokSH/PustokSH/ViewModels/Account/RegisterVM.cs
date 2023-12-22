using System.ComponentModel.DataAnnotations;

namespace PustokSH.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(320)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string SurName { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string UserName { get; set; }
    }
}
