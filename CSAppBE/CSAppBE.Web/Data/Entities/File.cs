namespace CSAppBE.Web.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class File : IEntity
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public byte[] Data { get; set; }

        public string FileType { get; set; }

        public string Description { get; set; }

        public DateTime? AddedDate { get; set; }

        public int Type { get; set; }
    }
}
