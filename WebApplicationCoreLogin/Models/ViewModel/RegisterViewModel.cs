using System.ComponentModel.DataAnnotations;

namespace WebApplicationCoreLogin.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Kullanıcı adı zorunludur.")]
        [StringLength(30,ErrorMessage ="Kullanıcı adı max.30 karakter olamlıdır.")]
        public string userName { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(16)]
        [DataType(DataType.Password)]

        public string password { get; set; } 
        [Required]
        [MinLength(6)]
        [MaxLength(16)]
        [DataType(DataType.Password)]
        [Compare(nameof(password))]
        public string password2 { get; set; }
    }
}
