namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FPLCapMst")]
    public partial class FPLCapMst
    {
        public long Id { get; set; }

        public decimal? BaseFPLBottom { get; set; }

        public decimal? BaseFPLTop { get; set; }

        public decimal? CapLow { get; set; }

        public decimal? CapHigh { get; set; }

        [StringLength(4)]
        public string BusinessYear { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
    }
}
