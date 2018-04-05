namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblRatingArea")]
    public partial class tblRatingArea
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long RatingAreaID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(4)]
        public string MarketCoverage { get; set; }

        [StringLength(30)]
        public string CountyName { get; set; }

        [StringLength(3)]
        public string ThreeDigitZipCode { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(2)]
        public string StateCode { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(4)]
        public string Businessyear { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }

        public virtual tblRatingAreaMst tblRatingAreaMst { get; set; }
    }
}
