namespace CSAppBE.Web.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class News : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Titulo")]
        public string Title { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? PublicatedDate { get; set; }

        [Display(Name = "Tipo")]
        public int Type { get; set; }
    }
}
