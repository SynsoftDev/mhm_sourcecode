namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JobMaster")]
    public partial class JobMaster
    {
        public JobMaster()
        {
            Cases = new HashSet<Case>();
            JobPlansMsts = new HashSet<JobPlansMst>();
        }

        [Key]
        [StringLength(13)]
        public string JobNumber { get; set; }
        [NotMapped]
        public string OldJobNumber { get; set; }

        public long? EmployerId { get; set; }

        [StringLength(500)]
        public string JobDesc { get; set; }

        [StringLength(500)]
        public string JobStatus { get; set; }

        public DateTime? JobDateStart { get; set; }

        public DateTime? JobDateEnd { get; set; }

        [StringLength(4)]
        public string JobYear { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [StringLength(5000)]
        public string EmailBodyText { get; set; }

        [StringLength(500)]
        public string EmailSubjText { get; set; }

        [StringLength(4000)]
        public string EmailSignText { get; set; }

        [StringLength(20)]
        public string JobRunStatus { get; set; }

        [StringLength(20)]
        public string JobType { get; set; }

        public DateTime? LastJobRunDt { get; set; }

        [StringLength(20)]
        public string LastJobRunStatus { get; set; }

        public long? LastJubRunUserID { get; set; }


        public int? InsuranceTypeId_Old { get; set; }

        [StringLength(5)]
        public string CaseZipCode { get; set; }

        [StringLength(13)]
        public string ComparisonJobNum { get; set; }

        [StringLength(13)]
        public string JobCopiedFrom { get; set; }

        public long? AcctMgr { get; set; }

        public DateTime? ExpectedCompletionDt { get; set; }

        public DateTime? PlanYearStartDt { get; set; }

        public DateTime? JobPlansSelectionLockedDt { get; set; }

        public bool? JobPlansSelectionLocked { get; set; }

        public DateTime? JobCensusImportDt { get; set; }

        public decimal? HRAMaxReimbursePrimary { get; set; }

        public decimal? HRAMaxReimburseDependent { get; set; }

        public decimal? HRADedLimitPrimary { get; set; }

        public decimal? HRADedLimitDependent { get; set; }

        public bool? HRACanCoverPremium { get; set; }

        public decimal? HRAReimburseRatePrimary { get; set; }

        public decimal? HRAReimburseRateDependent { get; set; }

        public bool? IsHSAMatch { get; set; }

        public decimal? HSAMatchLimit1 { get; set; }

        public decimal? HSAMatchRate1 { get; set; }

        public decimal? HSAMatchLimit2 { get; set; }

        public decimal? HSAMatchRate2 { get; set; }

        public decimal? HSAMatchLimit3 { get; set; }

        public decimal? HSAMatchRate3 { get; set; }

        public decimal? HSAMatchLimit4 { get; set; }

        public decimal? HSAMatchRate4 { get; set; }

        public DateTime? PlanYearEndDt { get; set; }

        [JsonIgnore]
        public virtual ICollection<Case> Cases { get; set; }
        [JsonIgnore]
        public virtual EmployerMst EmployerMst { get; set; }
        [JsonIgnore]
        public virtual ICollection<JobPlansMst> JobPlansMsts { get; set; }
    }
}

