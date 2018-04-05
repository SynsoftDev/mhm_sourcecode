using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Interfaces;
using MHMDal.Models;

namespace MHMDal.Repository
{
    public class MedicaidEligibilityRepo : IMedicaidEligibility
    {
        //MHM _context;

        public IEnumerable<MedicaidEligibility> GetMedicaidEligibility()
        {
            using (var _context = new MHM())
            {
                return _context.MedicaidEligibilities.ToList();
            }
        }
    }
}
