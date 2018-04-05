namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CasePlanResult
    {
        public long CasePlanResultId { get; set; }

        public long? CaseId { get; set; }

        public int? Rank { get; set; }

        //[NotMapped]
        public long PlanId { get; set; }
        //[NotMapped]
        public decimal PersonalHSAContribution { get; set; }
        //[NotMapped]
        public bool? ReferralForSpecialist { get; set; }

        [StringLength(200)]
        public string PlanIdIndiv1 { get; set; }

        [StringLength(20)]
        public string GovtPlanNumber { get; set; }

        [StringLength(4)]
        public string Year { get; set; }

        public decimal? GrossAnnualPremium { get; set; }
        //public decimal? GrossAnnualPremium { get; set; }

        public decimal? FederalSubsidy { get; set; }

        public decimal? NetAnnualPremium { get; set; }

        public decimal? MonthlyPremium { get; set; }

        public decimal? Copays { get; set; }

        public decimal? PaymentsToDeductibleLimit { get; set; }

        public decimal? CoinsuranceToOutOfPocketLimit { get; set; }

        public decimal? ContributedToYourHSAAccount { get; set; }

        public decimal? EmployerHRAReimbursement { get; set; }

        public decimal? TaxSavingFromHSAAccount { get; set; }

        public decimal? Medical { get; set; }

        public decimal? TotalPaid { get; set; }

        public decimal PaymentsByInsuranceCo { get; set; }

        public decimal? DeductibleSingle { get; set; }

        public decimal? DeductibleFamilyPerPerson { get; set; }

        public decimal? DeductibleFamilyPerGroup { get; set; }

        public decimal? OPLSingle { get; set; }

        public decimal? OPLFamilyPerPerson { get; set; }

        public decimal? OPLFamilyPerGroup { get; set; }

        public decimal? Coinsurance { get; set; }

        public decimal? WorstCase { get; set; }

        [StringLength(300)]
        public string MedicalNetwork { get; set; }

        [StringLength(100)]
        public string PlanName { get; set; }

        public decimal? HRAReimbursedAmt { get; set; }

        public decimal? TotalEmployerContribution_Pre { get; set; }

        public decimal? TotalEmployerContribution_Post { get; set; }

        public decimal? ExcludedAmount { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual Case Case { get; set; }
    }
}
