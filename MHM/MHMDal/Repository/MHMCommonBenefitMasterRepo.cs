using MHMDal.Interfaces;
using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMDal.Repository
{
    public class MHMCommonBenefitMasterRepo : IMHMCommonBenefitMaster
    {
        //MHM _context;

        public IEnumerable<MHMCommonBenefitsMst> GetMHMCommonBenefitMaster()
        {
            using (var _context = new MHM())
            {
                return _context.MHMCommonBenefitsMsts.ToList();
            }
        }
    }
}
