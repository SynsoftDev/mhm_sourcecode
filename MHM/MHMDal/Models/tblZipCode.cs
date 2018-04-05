namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblZipCode
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(5)]
        public string Zip { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string State { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(25)]
        public string County { get; set; }
                
        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        [StringLength(100)]
        public string City { get; set; }
    }
}
