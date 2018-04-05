namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblStateAbrev")]
    public partial class tblStateAbrev
    {
        public tblStateAbrev()
        {
            ChipEligibilities = new HashSet<ChipEligibility>();
            CSR_Rate_Mst = new HashSet<CSR_Rate_Mst>();
            IssuerMsts = new HashSet<IssuerMst>();
            MedicaidEligibilities = new HashSet<MedicaidEligibility>();
            MHMBenefitCostByAreaMsts = new HashSet<MHMBenefitCostByAreaMst>();
            PlanAttributeMsts = new HashSet<PlanAttributeMst>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string StateName { get; set; }

        [Key]
        [StringLength(2)]
        public string StateCode { get; set; }

        [StringLength(2)]
        public string FipsState { get; set; }

        [StringLength(10)]
        public string EntityType { get; set; }

        [StringLength(10)]
        public string IsoCode { get; set; }

        [Required]
        [StringLength(4)]
        public string Businessyear { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual ICollection<ChipEligibility> ChipEligibilities { get; set; }
        [JsonIgnore]
        public virtual ICollection<CSR_Rate_Mst> CSR_Rate_Mst { get; set; }
        [JsonIgnore]
        public virtual ICollection<IssuerMst> IssuerMsts { get; set; }
        [JsonIgnore]
        public virtual ICollection<MedicaidEligibility> MedicaidEligibilities { get; set; }
        [JsonIgnore]
        public virtual ICollection<MHMBenefitCostByAreaMst> MHMBenefitCostByAreaMsts { get; set; }
        [JsonIgnore]
        public virtual ICollection<PlanAttributeMst> PlanAttributeMsts { get; set; }
        
    }
}
