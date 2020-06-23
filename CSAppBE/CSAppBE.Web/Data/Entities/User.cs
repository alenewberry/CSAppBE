namespace CSAppBE.Web.Data.Entities
{
    using Microsoft.AspNetCore.Identity;
    using System;

    public class User : IdentityUser
    {
        public string Serial { get; set; }

        public string CUIT { get; set; }

        public string Name { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsActive { get; set; }

        public File Certificate { get; set; }
    }
}
