namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CensusImport")]
    public partial class CensusImport
    {
        public int Id { get; set; }

        [StringLength(13)]
        public string JobNumber { get; set; }

        [StringLength(25)]
        public string EmployeeFirstName { get; set; }

        [StringLength(35)]
        public string EmployeeLastName { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public DateTime CreateDatetime { get; set; }
    }
}
