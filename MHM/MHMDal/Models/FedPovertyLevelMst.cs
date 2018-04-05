namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FedPovertyLevelMst")]
    public partial class FedPovertyLevelMst
    {
        [Key]
        public long FPLId { get; set; }

        public int? HouseholdSize { get; set; }

        [Column(TypeName = "money")]
        public decimal? FPL { get; set; }

        [StringLength(4)]
        public string BusinessYear { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
    }
}
