using MHMDal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMDal.Interfaces
{
    public interface IMHMCommonBenefitMaster
    {
        IEnumerable<MHMCommonBenefitsMst> GetMHMCommonBenefitMaster();
    }
}
