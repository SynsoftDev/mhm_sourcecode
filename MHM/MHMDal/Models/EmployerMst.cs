namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmployerMst")]
    public partial class EmployerMst
    {
        public EmployerMst()
        {
            Applicants = new HashSet<Applicant>();
            JobMasters = new HashSet<JobMaster>();
        }

        [Key]
        public long EmployerId { get; set; }

        [Required]
        [StringLength(60)]
        public string EmployerName { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
        [JsonIgnore]
        public virtual ICollection<Applicant> Applicants { get; set; }
        [JsonIgnore]
        public virtual ICollection<JobMaster> JobMasters { get; set; }

    }
}
