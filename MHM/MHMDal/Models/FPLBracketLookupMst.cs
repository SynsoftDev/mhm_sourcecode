namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FPLBracketLookupMst")]
    public partial class FPLBracketLookupMst
    {
        public long Id { get; set; }

        public decimal? FPLBracketLookup { get; set; }

        [StringLength(30)]
        public string IncomePercentageFPL { get; set; }

        public int? SubPlanId { get; set; }

        [StringLength(4)]
        public string BusinessYear { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
    }
}
