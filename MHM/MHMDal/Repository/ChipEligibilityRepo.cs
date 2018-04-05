using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Interfaces;
using MHMDal.Models;

namespace MHMDal.Repository
{
    public class ChipEligibilityRepo : IChipEligibility
    {
        //MHM _context;
        
        public IEnumerable<ChipEligibility> GetChipEligibility()
        {
            using (var _context = new MHM())
            {
                return _context.ChipEligibilities.ToList();
            }
        }
    }
}
