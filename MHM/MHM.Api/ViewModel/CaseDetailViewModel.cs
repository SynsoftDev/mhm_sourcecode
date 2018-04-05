using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHM.Api.ViewModel
{
    public class CaseDetailViewModel
    {
      //  public long CaseID { get; set; }

        public string CaseTitle { get; set; }

        //[StringLength(4)]
        //public string Year { get; set; }

        //public long? IssuerID { get; set; }

        //public int? UsageID { get; set; }

        //public int? PlanID { get; set; }

        //public decimal? TaxRate { get; set; }

        //public decimal? HSAFunding { get; set; }

        //[StringLength(10)]
        //public string FPL { get; set; }

        //[Column(TypeName = "money")]
        //public decimal? MonthlySubsidy { get; set; }

        //[StringLength(20)]
        //public string HSALimit { get; set; }

     //   [StringLength(20)]
     //   public string HSAAmount { get; set; }

    //    [StringLength(20)]
    //    public string MAGIncome { get; set; }

     //   [Column(TypeName = "money")]
    //    public decimal? TotalMedicalUsage { get; set; }

     //   public bool? Welness { get; set; }

      //  public DateTime CreatedDateTime { get; set; }

     //   public DateTime? ModifiedDateTime { get; set; }

     //   public long Createdby { get; set; }

     //   public long? ModifiedBy { get; set; }

     //   [StringLength(7)]
     //   public string ZipCode { get; set; }

     //   public string Notes { get; set; }

       // public bool? PreviousYrHSA { get; set; }

      //  public virtual ApplicantViewModel Applicant { get; set; }

    }

    public class ApplicantViewModel
    {

        public long ApplicantID { get; set; }

        public long? EmployerId { get; set; }

        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [StringLength(20)]
        public string CurrentPlan { get; set; }

        [StringLength(20)]
        public string CurrentPremium { get; set; }

        public bool? Origin { get; set; }

        [StringLength(200)]
        public string Street { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        public int Zip { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }

        public long? Createdby { get; set; }

        public long? ModifiedBy { get; set; }
    }

}
