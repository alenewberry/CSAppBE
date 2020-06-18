
namespace CSAppBE.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name = "Razón Social")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Número de Serie")]
        public string Serial { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password")]
        public string Confirm { get; set; }
    }
}
