namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MHMBenefitCostByAreaMst")]
    public partial class MHMBenefitCostByAreaMst
    {
        public long Id { get; set; }

        public long MHMBenefitId { get; set; }

        public long? RatingAreaID { get; set; }

        [Column(TypeName = "money")]
        public decimal? MHMBenefitCost { get; set; }

        public int? MHMBenefitUnitOfMeasure { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [StringLength(2)]
        public string StateCode { get; set; }
       
        public virtual tblStateAbrev tblStateAbrev { get; set; }
       
        public virtual MHMCommonBenefitsMst MHMCommonBenefitsMst { get; set; }
    }
}
