namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CaseStatusMst")]
    public partial class CaseStatusMst
    {
        public CaseStatusMst()
        {
            Cases = new HashSet<Case>();
        }

        [Key]
        public int StatusId { get; set; }

        [Required]
        //[StringLength(20)]
        public string StatusCode { get; set; }

        public bool Editable { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        public int? Sortby { get; set; }

        public int? Parent { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(100)]
        public string Descr { get; set; }
        [JsonIgnore]
        public virtual ICollection<Case> Cases { get; set; }
    }
}
