using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMDal.Interfaces
{
    public interface IRule
    {
        void ExecuteRule(Dictionary<string, object> InObject, out Dictionary<string, object> OutObject);
    }
}
