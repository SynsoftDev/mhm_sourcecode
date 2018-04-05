namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Case")]
    public partial class Case
    {
        public Case()
        {
            CasePlanResults = new HashSet<CasePlanResult>();
            Families = new HashSet<Family>();
        }
        [Key]
        public long CaseID { get; set; }

        public long? ApplicantID { get; set; }

        [StringLength(2000)]
        public string CaseTitle { get; set; }

        [StringLength(4)]
        public string Year { get; set; }

        public long? IssuerID { get; set; }

        public int? UsageID { get; set; }

        public int? PlanID { get; set; }

        public decimal? TaxRate { get; set; }

        public decimal? HSAFunding { get; set; }

        public decimal? FPL { get; set; }

        public decimal? MonthlySubsidy { get; set; }

        public decimal? HSALimit { get; set; }

        public decimal? HSAAmount { get; set; }

        public decimal? MAGIncome { get; set; }

        public decimal? TotalMedicalUsage { get; set; }

        public bool? Welness { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        public string Notes { get; set; }

        public bool? PreviousYrHSA { get; set; }

        public long? CaseReferenceId { get; set; }

        [StringLength(30)]
        public string CountyName { get; set; }

        public long? RatingAreaId { get; set; }

        public bool IsSubsidy { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        public int? StatusId { get; set; }

        [StringLength(13)]
        public string JobNumber { get; set; }

        public string CaseSource { get; set; }

        [StringLength(25)]
        public string CaseJobRunStatus { get; set; }

        public DateTime? CaseJobRunDt { get; set; }

        public long? CaseJobRunUserID { get; set; }

        [StringLength(200)]
        public string CaseJobRunMsg { get; set; }

        public bool? InProcessStatus { get; set; }


        public DateTime? DedBalAvailDate { get; set; }
        public decimal? DedBalAvailToRollOver { get; set; }
        public int? TierIntention { get; set; }

        public bool? PrimaryCase { get; set; }

        public bool? AlternateCase { get; set; }


        public virtual Applicant Applicant { get; set; }
        //[JsonIgnore]
        public virtual JobMaster JobMaster { get; set; }
        [JsonIgnore]
        public virtual tblRatingAreaMst tblRatingAreaMst { get; set; }

        public virtual CaseStatusMst CaseStatusMst { get; set; }

        public virtual IssuerMst IssuerMst { get; set; }

        public virtual ICollection<CasePlanResult> CasePlanResults { get; set; }

        public virtual ICollection<Family> Families { get; set; }

        public long? Agent { get; set; }

        public DateTime? InterviewDate { get; set; }

    }
}
