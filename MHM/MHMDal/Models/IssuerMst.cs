namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IssuerMst")]
    public partial class IssuerMst
    {
        public IssuerMst()
        {
            Cases = new HashSet<Case>();
            MHMBenefitMappingMsts = new HashSet<MHMBenefitMappingMst>();
            PlanAttributeMsts = new HashSet<PlanAttributeMst>();
        }

        public long Id { get; set; }

        [StringLength(10)]
        public string IssuerCode { get; set; }

        [StringLength(250)]
        public string IssuerName { get; set; }

        [StringLength(10)]
        public string MappingNumber { get; set; }

        [StringLength(100)]
        public string Abbreviations { get; set; }

        public bool? Status { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [StringLength(2)]
        public string StateCode { get; set; }
        [JsonIgnore]
        public virtual ICollection<Case> Cases { get; set; }
        [JsonIgnore]
        public virtual tblStateAbrev tblStateAbrev { get; set; }
        [JsonIgnore]
        public virtual ICollection<MHMBenefitMappingMst> MHMBenefitMappingMsts { get; set; }
        [JsonIgnore]
        public virtual ICollection<PlanAttributeMst> PlanAttributeMsts { get; set; }
        [NotMapped]
        public virtual List<BenefitIdsViewModel> BenefitIds { get; set; }
    }

    public class BenefitIdsViewModel
    {
        public BenefitIdsViewModel()
        { }

        public string BenefitName { get; set; }
        public int? IssuerBenefitId { get; set; }
        public long? CarrierId { get; set; }
    }

}
