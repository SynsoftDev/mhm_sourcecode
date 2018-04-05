namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlanBenefitMst")]
    public partial class PlanBenefitMst
    {
        public long Id { get; set; }

        [StringLength(100)]
        public string BenefitKey_Old { get; set; }

        [StringLength(4)]
        public string BusinessYear { get; set; }

        [StringLength(250)]
        public string BenefitName { get; set; }

        [StringLength(150)]
        public string CopayInnTier1Desc { get; set; }

        [StringLength(1)]
        public string CopayInnTier1Type_Old { get; set; }

        [StringLength(250)]
        public string CoinsInnTier1Desc { get; set; }

        public decimal? CopayInnTier1 { get; set; }

        public decimal? CoinsInnTier1 { get; set; }

        public bool? IsEHB { get; set; }

        public bool? IsCovered { get; set; }

        public bool? IsSubjToDedTier1 { get; set; }

        public bool? IsExclFromInnMOOP { get; set; }

        [StringLength(50)]
        public string MarketConverage_Old { get; set; }

        [StringLength(50)]
        public string SourceName { get; set; }

        public int? VersionNum_Old { get; set; }

        [StringLength(50)]
        public string StandardComponentId_Old { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [StringLength(2)]
        public string StateCode_Old { get; set; }

        public int? BenefitNum { get; set; }

        [StringLength(20)]
        public string PlanId { get; set; }

        public long? MHMBenefitId { get; set; }

        public decimal? MaxCoinsInnTier1Amt { get; set; }

        public decimal? CopayInnTier2 { get; set; }

        public decimal? CoinsInnTier2 { get; set; }

        public decimal? MaxCoinsInnTier2Amt { get; set; }

        public decimal? CopayOutofNet { get; set; }

        public decimal? CoinsOutofNet { get; set; }

        public bool? QuantLimitOnSvc { get; set; }

        public decimal? LimitQty { get; set; }

        [StringLength(32)]
        public string LimitUnit { get; set; }

        public bool? IsExclFromOonMOOP { get; set; }

        [StringLength(250)]
        public string Exclusions { get; set; }

        public decimal? BenefitDeductible { get; set; }

        [StringLength(50)]
        public string CostSharingType1 { get; set; }

        [StringLength(50)]
        public string CostSharingType2 { get; set; }

        public decimal? CoinsMaxAmt { get; set; }

        public decimal? MaxQtyBeforeCoPay { get; set; }

        public string PlanBenNotes { get; set; }

        public string SBCText { get; set; }

        [NotMapped]
        public string IssuerName { get; set; }

        [NotMapped]
        public long? IssuerId { get; set; }

        //[NotMapped]
        //public Int32? Unassign { get; set; }

        [NotMapped]
        public string PlanMarketingName { get; set; }

        [JsonIgnore]
        public virtual PlanAttributeMst PlanAttributeMst { get; set; }

    }
}
