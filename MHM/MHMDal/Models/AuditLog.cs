namespace MHMDal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuditLog")]
    public partial class AuditLog
    {
        public long AuditLogId { get; set; }

        public long UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime LoginDate { get; set; }

        public TimeSpan LoginTime { get; set; }

        public short LoginStatus { get; set; }

        [StringLength(4000)]
        public string LogText { get; set; }

        public virtual User User { get; set; }
    }
}
