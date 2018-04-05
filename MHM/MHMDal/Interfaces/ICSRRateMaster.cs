using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;

namespace MHMDal.Interfaces
{
    public interface ICSRRateMaster
    {
        IEnumerable<CSR_Rate_Mst> GetCSRRateMaster();
    }
}
