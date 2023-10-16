using ABP.Domain.Indicator;
using ABP.Domain.Report;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.District
{


    /// <summary>
    /// Interface IDistrictRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface IDistrictRepository : IDisposable, IRepository
    {

        /// <summary>
        /// Get Login Detail
        /// </summary>
        /// <param name="LoginClosure">Login Request ParaMeter</param>
        /// <returns><seealso cref="Domain.District.District"/></returns>
        /// 

        string InsertDistrict(Domain.District.District Model);
        Task<IEnumerable<Domain.District.District>> GetDistrictById(int DISTRICTID);

        string DeleteDistrict(Domain.District.District Model);
        Task<IEnumerable<Domain.District.District>> GetDistrictDetails(Domain.District.District search);
        Task<IEnumerable<Domain.Report.DistReport>> GetDistrictAsync();
        Task<IEnumerable<Domain.Report.DistReport>> GetByDistrictDataDetails(int year, int FREQUENCYNO);
        Task<IEnumerable<Domain.Report.DistReport>> IndicatorScore(int distcode, int year, int month);
        Task<IEnumerable<Domain.Report.DistReport>> getblockdata(int distcode);
        
        Task<IEnumerable<Domain.Block.Block>> GetBlockByDist(int DistCode);
        Task<IEnumerable<Domain.Block.Block>> GetUserAndBlockByDist(int DistCode);
        Task<IEnumerable<Domain.Login.Login>> GetByBlockDetails(int DistCode);

        Task<IEnumerable<Domain.Login.Login>> GetByDistrictDetails();

        Task<IEnumerable<Domain.Indicator.IndicatorValueScore>> GetAppnoForCalculation(int distcode);

        Task<int> INSERTSCORE(Domain.Indicator.IndicatorScoreXML ivs);
        Task<int> YEARLYINSERTSCORE(Domain.Indicator.IndicatorScoreXML ivs);
        Task<int> INSERTAnalyticData(int Pdist,int Pblock, int Pyear,int Pmonth,int Pfrquency);
        Task<int> INSERTAnalyticDataCopy(int Pdist, int Pblock, int Pyear, int Pmonth, int Pfrquency);
        Task<int> INSERTAnalyticDataCopyYearly(int Pdist, int Pblock, int Pyear, int Pmonth, int Pfrquency);
     
        Task<int> INSERTAnalyticDataYearly(int Pdist, int Pblock, int year, int Pfrquency);
        Task<IEnumerable<Domain.Indicator.IndicatorValueScore>> GetAvgIndicatorScore(int distcode);
        Task<IEnumerable<Domain.Designation.Designation>> GetUserAndDesgByDept(int Deptid);
        Task<IEnumerable<Domain.Designation.Designation>> GetUserAndColByDist(int DistId);
        Task<int> UPDATESCORQUERYE(Domain.Indicator.IndicatorValueScore ivs);
        Task<IEnumerable<Domain.Block.Block>> GetDistData(int DistCode);
       
        Task<IEnumerable<Domain.Report.Report>> GetDPMonCount();
        Task<int> INSERTAspirationalDetail(Domain.Report.Report lr);
        Task<IEnumerable<Domain.Report.Report>> ViewAspirationalBlock(Domain.Report.Report rpt);


        Task<IEnumerable<Domain.Report.Report>> GetAspirationalid(int Blockid);

        Task<IEnumerable<Domain.Report.Report>> GetMappedeBlockByDist(int DistCode);
        string GetMobNo(string MobNo);
    }

}
