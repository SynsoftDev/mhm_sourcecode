using MHMDal.Interfaces;
using MHMDal.Models;
using MHMDal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHMDal;

namespace MHMBLL
{
    public class StateandRatingArea
    {
        MHMCache MHMCache = new MHMCache();
        IqryZipCodeToRatingAreas qryZipCodeToRatingAreasMaster = new qryZipCodeToRatingAreasRepo();
        
        public void GetStateCodeandRatingArea(string zipCode, string CountyName, out long RatingAreaId, out string StateCode)
        {
            var qryZipCodeToRatingAreasMasterFromCache = MHMCache.GetMyCachedItem("qryZipCodeToRatingAreasMaster");

            try
            {
                if (qryZipCodeToRatingAreasMasterFromCache != null)
                {
                    StateCode = ((IEnumerable<qryZipCodeToRatingArea>)qryZipCodeToRatingAreasMasterFromCache).Where(r => r.Zip == zipCode && r.CountyName == CountyName).FirstOrDefault().StateCode;
                }
                else
                {
                    var qryZipCodeToRatingAreaList = qryZipCodeToRatingAreasMaster.GetqryZipCodeToRatingArea().ToList();
                    MHMCache.AddToMyCache("qryZipCodeToRatingAreasMaster", qryZipCodeToRatingAreaList, MyCachePriority.Default);
                    StateCode = qryZipCodeToRatingAreaList.Where(r => r.Zip == zipCode && r.CountyName == CountyName).FirstOrDefault().StateCode;
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("State cannot be found. ");
            }

            qryZipCodeToRatingAreasMasterFromCache = MHMCache.GetMyCachedItem("qryZipCodeToRatingAreasMaster");
            try
            {
                if (qryZipCodeToRatingAreasMasterFromCache != null)
                {
                    RatingAreaId = (long)((IEnumerable<qryZipCodeToRatingArea>)qryZipCodeToRatingAreasMasterFromCache).Where(r => r.Zip == zipCode && r.CountyName == CountyName && (r.MarketCoverage == "Both" || r.MarketCoverage == "Indi")).FirstOrDefault().RatingAreaID;

                }
                else
                {
                    var qryZipCodeToRatingAreaList = qryZipCodeToRatingAreasMaster.GetqryZipCodeToRatingArea().ToList();
                    MHMCache.AddToMyCache("qryZipCodeToRatingAreasMaster", qryZipCodeToRatingAreaList, MyCachePriority.Default);
                    RatingAreaId = (long)qryZipCodeToRatingAreaList.Where(r => r.Zip == zipCode && r.CountyName == CountyName && (r.MarketCoverage == "Both" || r.MarketCoverage == "Indi")).FirstOrDefault().RatingAreaID;
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Rating Id is not mapped to county.");
            }
        }
    }
}
