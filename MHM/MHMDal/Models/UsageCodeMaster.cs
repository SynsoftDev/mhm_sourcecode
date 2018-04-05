namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UsageCodeMaster")]
    public partial class UsageCodeMaster
    {
        [Key]
        public int UsagaId { get; set; }

        [StringLength(50)]
        public string UsageType { get; set; }
    }
}
