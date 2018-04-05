using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMDal.Models
{
    public class FamilySheetResult
    {
        public long Copays { get; set; }
        public long PaymentsuptoDeductible { get; set; }
        public long Coinsurance { get; set; }
        public long PaymentsByInsurance { get; set; }
        public long TotalOOPCost { get; set; }
        public long ExcludedAmount { get; set; }
        public decimal FamilyHRAReimbursementTotal { get; set; }
        public Dictionary<string, decimal?> Limits { get; set; }
    }
}
