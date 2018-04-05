using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Interfaces;
using MHMDal.Models;
using MHMDal.Repository;

namespace MHMBLL
{
    public class DefaultPlanRule : IRule
    {

        public void ExecuteRule(Dictionary<string, object> InObject, out Dictionary<string, object> OutObject)
        {
            try
            {
                OutObject = new Dictionary<string, object>();
                OutObject["DefaultPlanIdSub"] = Constants.DefaultPlanNumber;
            }
            catch (Exception ex)
            {

                throw new Exception("Defualt Plan Rule Error");
            }

        }

    }
}
