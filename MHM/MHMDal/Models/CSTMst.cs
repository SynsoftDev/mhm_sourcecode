namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CSTMst")]
    public partial class CSTMst
    {
        public long Id { get; set; }

        [StringLength(30)]
        public string Key { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(10)]
        public string Status { get; set; }
    }
}
