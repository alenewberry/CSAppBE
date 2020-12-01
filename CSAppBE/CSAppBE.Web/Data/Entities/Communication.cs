namespace CSAppBE.Web.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Communication : IEntity
    {
        public int Id { get; set; }

        public long CommunicationId { get; set; }

        public string Cuit { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PublishedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }

        public string PublicSystemId { get; set; }

        [Display(Name = "Sistema Publicador")]
        public string PublicSystemDesc { get; set; }

        
        public int Status { get; set; }

        [Display(Name = "Estado")]
        public string StatusDesc { get; set; }

        [Display(Name = "Asunto")]
        public string Subject { get; set; }

        public int Priority { get; set; }

        [Display(Name = "Adjunto")]
        public bool Attach { get; set; }

        public string Ref1 { get; set; }

        public string Ref2 { get; set; }

        public Client Client { get; set; }
    }
}
