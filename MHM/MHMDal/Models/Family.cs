namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Family")]
    public partial class Family
    {
        public Family()
        {
            BenefitUserDetails = new HashSet<BenefitUserDetail>();
            Criticalillnesses = new HashSet<Criticalillness>();
        }

        public long FamilyID { get; set; }

        public long CaseNumId { get; set; }

        [StringLength(56)]
        public string Gender { get; set; }

        [StringLength(128)]
        public string DOB { get; set; }

        public int? Age { get; set; }

        public bool? Smoking { get; set; }

        public decimal? TotalMedicalUsage { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        public bool IsPrimary { get; set; }

        [StringLength(2)]
        public string Individual { get; set; }
        
        public virtual ICollection<BenefitUserDetail> BenefitUserDetails { get; set; }
        [JsonIgnore]
        public virtual Case Case { get; set; }

        public virtual ICollection<Criticalillness> Criticalillnesses { get; set; }
    }
}
