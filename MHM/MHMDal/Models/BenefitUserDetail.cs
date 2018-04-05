namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BenefitUserDetail")]
    public partial class BenefitUserDetail
    {
        [Key]
        public long BenefitUseDetailId { get; set; }

        public long FamilyID { get; set; }

        public long MHMMappingBenefitId { get; set; }

        public decimal? UsageCost { get; set; }

        public decimal? UsageQty { get; set; }

        [StringLength(4000)]
        public string UsageNotes { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual Family Family { get; set; }
        [JsonIgnore]
        public virtual MHMCommonBenefitsMst MHMCommonBenefitsMst { get; set; }
    }
}
