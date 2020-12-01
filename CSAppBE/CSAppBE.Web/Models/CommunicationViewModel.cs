using System.ComponentModel.DataAnnotations;

namespace CSAppBE.Web.Models
{
    public class CommunicationViewModel
    {
        [Display(Name = "Nombre del Certificado")]
        public string FileName { get; set; }

        [Display(Name = "Certificado")]
        public byte[] Data { get; set; }

        [Display(Name = "Mensaje")]
        public string Message { get; set; }

        [Display(Name = "Asunto")]
        public string Subject { get; set; }

    }
}
