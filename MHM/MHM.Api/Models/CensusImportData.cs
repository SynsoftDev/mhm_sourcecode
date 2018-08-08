using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHM.Api.Models
{
    public class CensusImportData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string MAGI { get; set; }
        public string CurrentPlan { get; set; }
        public string UsageID { get; set; }
        public string HireDate { get; set; }
        public string EREmpId { get; set; }
        public string JobTitle { get; set; }
        public string CaseTitle { get; set; }
        public string Wellness { get; set; }
    }
}