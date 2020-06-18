namespace CSAppBE.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "Razón Social")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Número de Serie")]
        public string Serial { get; set; }
    }

}
