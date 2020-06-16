namespace CSAppBE.Web.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    public class Client
    {
        public int Id { get; set; }

        public string CUIT { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

    }
}
