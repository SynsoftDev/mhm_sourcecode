namespace MHMDal.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Applicant")]
    public partial class Applicant
    {
        public Applicant()
        {
            Cases = new HashSet<Case>();
        }

        public long ApplicantID { get; set; }

        public long? EmployerId { get; set; }

        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [StringLength(256)]
        public string CurrentPlan { get; set; }

        [StringLength(256)]
        public string CurrentPremium { get; set; }

        public bool? Origin { get; set; }

        [StringLength(2000)]
        public string Street { get; set; }

        [Required]
        [StringLength(2000)]
        public string City { get; set; }

        [Required]
        [StringLength(2000)]
        public string State { get; set; }

        [Required]
        public string Zip { get; set; }

        public string Email { get; set; }

        [Required]
        public string Mobile { get; set; }

        public int? InsuranceTypeId { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [NotMapped]
        public string EmployerName { get; set; }

        public DateTime? HireDate { get; set; }

        [StringLength(20)]
        public string EREmpId { get; set; }

        [StringLength(25)]
        public string JobTitle { get; set; }

        [JsonIgnore]
        public virtual EmployerMst EmployerMst { get; set; }
        [JsonIgnore]
        public virtual ICollection<Case> Cases { get; set; }
    }
}
