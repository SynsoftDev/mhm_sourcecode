namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Criticalillness")]
    public partial class Criticalillness
    {
        public long Id { get; set; }

        public long FamilyID { get; set; }

        public long IllnessId { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual Family Family { get; set; }
        [JsonIgnore]
        public virtual CriticalillnessMst CriticalillnessMst { get; set; }
    }
}
