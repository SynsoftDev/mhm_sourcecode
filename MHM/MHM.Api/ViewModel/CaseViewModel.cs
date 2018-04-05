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

    public class CaseViewModel : Response
    {
        public virtual ICollection<CaseListViewModel> Cases { get; set; }
    }

    public class CaseListViewModel
    {
        public long CaseID { get; set; }

        public string CaseTitle { get; set; }

        public string Carrier { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string ApplicantName { get; set; }

        public string ComapnyName { get; set; }

        public string UsageType { get; set; }

        public string EmployerName { get; set; }

        public string MobileNo { get; set; }

        public string JobNumber { get; set; }

        public string StatusCode { get; set; }

        public bool Editable { get; set; }

        public string BusinessYear { get; set; }

        public string CaseSource { get; set; }

        public string CreatedBy { get; set; }

        public string Agent { get; set; }

        public Boolean? PrimaryCase { get; set; }

        public Boolean? AlternateCase { get; set; }
    }

}
