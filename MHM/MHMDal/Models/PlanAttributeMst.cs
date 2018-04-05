namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlanAttributeMst")]
    public partial class PlanAttributeMst
    {
        public PlanAttributeMst()
        {
            JobPlansMsts = new HashSet<JobPlansMst>();
            PlanBenefitMsts = new HashSet<PlanBenefitMst>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string PlanId { get; set; }

        [StringLength(50)]
        public string StandardComponentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string BusinessYear { get; set; }

        public long? CarrierId { get; set; }

        [StringLength(50)]
        public string MrktCover { get; set; }

        public bool? DentalOnlyPlan { get; set; }

        [StringLength(250)]
        public string PlanMarketingName { get; set; }

        [StringLength(50)]
        public string HIOSProductId { get; set; }

        [StringLength(50)]
        public string ServiceAreaId { get; set; }

        public int? PlanType { get; set; }

        [StringLength(50)]
        public string MetalLevel { get; set; }

        [StringLength(50)]
        public string QHPNonQHPTypeId { get; set; }

        public bool IsHSAEligible { get; set; }

        public bool IsHRAeligible { get; set; }

        [StringLength(50)]
        public string HSAOrHRAEmployerContribution_Old { get; set; }

        public decimal? HSAOrHRAEmployerContributionAmount_Old { get; set; }

        public decimal? SpecialtyDrugMaximumCoinsurance { get; set; }

        public DateTime? PlanEffictiveDate { get; set; }

        public DateTime? PlanExpirationDate { get; set; }

        [StringLength(250)]
        public string CSRVariationType { get; set; }

        public bool? MedicalDrugDeductiblesIntegrated { get; set; }

        public bool? MedicalDrugMaximumOutofPocketIntegrated { get; set; }

        public decimal? TEHBInnTier1IndividualMOOP { get; set; }

        public decimal? TEHBInnTier1FamilyPerGroupMOOP { get; set; }

        public decimal? TEHBOutOfNetFamilyPerGroupMOOP { get; set; }

        public decimal? TEHBDedInnTier1Individual { get; set; }

        public decimal? TEHBDedInnTier1FamilyPerGroup { get; set; }

        public decimal? TEHBDedOutOfNetFamilyPerGroup { get; set; }

        public decimal? TEHBDedInnTier1Coinsurance { get; set; }

        [StringLength(250)]
        public string URLForSummaryofBenefitsCoverage { get; set; }

        [StringLength(250)]
        public string FormularyURL { get; set; }

        [StringLength(250)]
        public string NetworkURL { get; set; }

        [StringLength(250)]
        public string PlanBrochure { get; set; }

        public int? BenefitPackageId { get; set; }

        [StringLength(10)]
        public string PlanNumber { get; set; }

        [StringLength(10)]
        public string MappingNumber_Old { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [StringLength(200)]
        public string GroupName { get; set; }

        public long? EmployerId_Old { get; set; }

        public int? InsuranceType { get; set; }

        public bool? OpenForEnrollment { get; set; }

        public DateTime? OpenForEnrollment_ChangedDate { get; set; }

        [StringLength(2)]
        public string StateCode { get; set; }

        public bool? NonEmbeddedOOPLimits { get; set; }

        public int ApprovalStatus { get; set; }

        public decimal? TEHBDedInnTier1FamilyPerPerson { get; set; }

        public decimal? TEHBDedInnTier2Coinsurance { get; set; }

        public decimal? TEHBDedInnTier2FamilyPerGroup { get; set; }

        public decimal? TEHBDedInnTier2FamilyPerPerson { get; set; }

        public decimal? TEHBDedInnTier2Individual { get; set; }

        public decimal? TEHBInnTier1FamilyPerPersonMOOP { get; set; }

        public decimal? TEHBInnTier2FamilyPerGroupMOOP { get; set; }

        public decimal? TEHBInnTier2FamilyPerPersonMOOP { get; set; }

        public decimal? TEHBInnTier2IndividualMOOP { get; set; }

        public decimal? MEHBDedInnTier1Coinsurance { get; set; }

        public decimal? MEHBDedInnTier1FamilyPerGroup { get; set; }

        public decimal? MEHBDedInnTier1FamilyPerPerson { get; set; }

        public decimal? MEHBDedInnTier1Individual { get; set; }

        public decimal? MEHBDedInnTier2Coinsurance { get; set; }

        public decimal? MEHBDedInnTier2FamilyPerGroup { get; set; }

        public decimal? MEHBDedInnTier2FamilyPerPerson { get; set; }

        public decimal? MEHBDedInnTier2Individual { get; set; }

        public decimal? MEHBInnTier1FamilyPerGroupMOOP { get; set; }

        public decimal? MEHBInnTier1FamilyPerPersonMOOP { get; set; }

        public decimal? MEHBInnTier1IndividualMOOP { get; set; }

        public decimal? MEHBInnTier2FamilyPerGroupMOOP { get; set; }

        public decimal? MEHBInnTier2FamilyPerPersonMOOP { get; set; }

        public decimal? MEHBInnTier2IndividualMOOP { get; set; }

        public decimal? DEHBDedInnTier1Coinsurance { get; set; }

        public decimal? DEHBDedInnTier1FamilyPerGroup { get; set; }

        public decimal? DEHBDedInnTier1FamilyPerPerson { get; set; }

        public decimal? DEHBDedInnTier1Individual { get; set; }

        public decimal? DEHBDedInnTier2Coinsurance { get; set; }

        public decimal? DEHBDedInnTier2FamilyPerGroup { get; set; }

        public decimal? DEHBDedInnTier2FamilyPerPerson { get; set; }

        public decimal? DEHBDedInnTier2Individual { get; set; }

        public decimal? DEHBInnTier1FamilyPerGroupMOOP { get; set; }

        public decimal? DEHBInnTier1FamilyPerPersonMOOP { get; set; }

        public decimal? DEHBInnTier1IndividualMOOP { get; set; }

        public decimal? DEHBInnTier2FamilyPerGroupMOOP { get; set; }

        public decimal? DEHBInnTier2FamilyPerPersonMOOP { get; set; }

        public decimal? DEHBInnTier2IndividualMOOP { get; set; }

        public string PlanFormalName { get; set; }

        public string PlanNotes { get; set; }

        public bool? ReferralForSpecialist { get; set; }

        [JsonIgnore]
        public virtual InsuranceType InsuranceType1 { get; set; }
        [JsonIgnore]
        public virtual IssuerMst IssuerMst { get; set; }
        [JsonIgnore]
        public virtual ICollection<JobPlansMst> JobPlansMsts { get; set; }
        [JsonIgnore]
        public virtual tblStateAbrev tblStateAbrev { get; set; }
        [JsonIgnore]
        public virtual PlanMaster PlanMaster { get; set; }
        [JsonIgnore]
        public virtual ICollection<PlanBenefitMst> PlanBenefitMsts { get; set; }

        [NotMapped]
        public long Count { get; set; }
        [NotMapped]
        public string IssuerName { get; set; }
        [NotMapped]
        public long OldPlanId { get; set; }
        [NotMapped]
        public int UnassignedBen { get; set; }
    }
}
