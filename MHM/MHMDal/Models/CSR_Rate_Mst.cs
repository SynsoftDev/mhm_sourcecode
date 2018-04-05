namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CSR_Rate_Mst
    {
        [NotMapped]
        public decimal? EHBRate { get; set; }

        public long Id { get; set; }

        [StringLength(20)]
        public string PlanID { get; set; }

        public long? RatingAreaId { get; set; }

        [StringLength(50)]
        public string Age { get; set; }

        [StringLength(50)]
        public string MetalLevel_Old { get; set; }

        [StringLength(250)]
        public string PlanMarketingName_Old { get; set; }

        public decimal? IndividualRate { get; set; }

        public decimal? IndividualTobaccoRate { get; set; }

        public DateTime? RateEffectiveDate { get; set; }

        [StringLength(50)]
        public string MrktCover_Old { get; set; }

        [StringLength(4)]
        public string BusinessYear { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        public decimal? EHBPercentTotalPremium { get; set; }

        public decimal? GrpCostAmt { get; set; }

        public decimal? GrpEmplrPremAmt { get; set; }

        public decimal? GrpHSAAmt { get; set; }

        public decimal? GrpCashAmt { get; set; }

        [StringLength(2)]
        public string StateCode_Old { get; set; }

        public decimal? HRAMaxReimbursementAmt { get; set; }
        [JsonIgnore]
        public virtual tblRatingAreaMst tblRatingAreaMst { get; set; }
        [JsonIgnore]
        public virtual tblStateAbrev tblStateAbrev { get; set; }

    }
}
