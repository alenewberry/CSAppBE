namespace CSAppBE.Web.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string CUIT { get; set; }

        [MaxLength(80)]
        [Required]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

    }
}
