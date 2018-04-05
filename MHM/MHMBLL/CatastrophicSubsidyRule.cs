using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal.Models;
using MHMDal.Interfaces;
using MHMDal.Repository;

namespace MHMBLL
{
    public class CatastrophicSubsidyRule : IRule
    {
        public void ExecuteRule(Dictionary<string, object> InObject, out Dictionary<string, object> OutObject)
        {
            var MetalLevel = InObject["MetalLevel"].ToString();
            var SubsidyAmount = (decimal)InObject["SubsidyAmount"];
            if (MetalLevel == "Catastrophic")
            {
                SubsidyAmount = 0;
            }
            OutObject = new Dictionary<string, object>();
            OutObject.Add("SubsidyAmount", SubsidyAmount);
        }
    }
}
