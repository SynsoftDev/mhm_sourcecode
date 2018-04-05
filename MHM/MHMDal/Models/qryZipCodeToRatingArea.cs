namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("qryZipCodeToRatingArea")]
    public partial class qryZipCodeToRatingArea
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
        [StringLength(25)]
        public string CountyName { get; set; }

        [StringLength(2)]
        public string StateCode { get; set; }

        public string StateName { get; set; }

        public long StateId { get; set; }

        public long? RatingAreaID { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string RatingAreaName { get; set; }


        [StringLength(4)]
        public string MarketCoverage { get; set; }
    }
}
