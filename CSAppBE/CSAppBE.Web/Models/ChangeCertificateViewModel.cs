namespace CSAppBE.Web.Models
{
    using CSAppBE.Web.Data.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    public class ChangeCertificateViewModel
    {
        [Display(Name = "Certificado")]
        public File Certificate { get; set; }

        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
