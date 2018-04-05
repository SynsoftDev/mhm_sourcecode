namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("qryZipStateCounty")]
    public partial class qryZipStateCounty
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(5)]
        public string County { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(5)]
        public string Zip { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(2)]
        public string State { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(25)]
        public string CountyName { get; set; }

        [StringLength(30)]
        public string StateName { get; set; }

        [StringLength(3)]
        public string ZIP3 { get; set; }

        [StringLength(2)]
        public string StateCode { get; set; }

    }
}
