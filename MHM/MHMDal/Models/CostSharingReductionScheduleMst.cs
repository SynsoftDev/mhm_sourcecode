namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CostSharingReductionScheduleMst")]
    public partial class CostSharingReductionScheduleMst
    {
        public long Id { get; set; }

        [StringLength(50)]
        public string IncomePercentageFPL { get; set; }

        public decimal? ActuarialValue { get; set; }

        public decimal? OOPMV_Individual { get; set; }

        public decimal? OOPMV_Family { get; set; }

        [StringLength(4)]
        public string BusinessYear { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
    }
}
