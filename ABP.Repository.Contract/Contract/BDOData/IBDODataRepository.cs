using ABP.Domain.Indicator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Repository.Contract.Contract.BDOData
{
    /// <summary>
    /// Interface IBDODataRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface IBDODataRepository
    {
        /// <summary>
        /// Get DataPoint Detail
        /// </summary>
        /// <param name="ManageBDOData">DataPoint ParaMeter</param>
        /// <returns><seealso cref="Domain.BDOData.BDOData"/></returns>

        //string AddBDOData(Domain.DataPoint.DataPoint dp);
        //void BDODataSubmission(Domain.DataPoint.DataPoint dp);
        string AddBDOData(XElement xml,XElement xml2, Domain.DataPoint.DataPoint dp, int BLOCKID, int FREQUENCYNO, int userid);
        string UpdateBDOData(XElement xml, XElement xml2, Domain.DataPoint.DataPoint dp, int BLOCKID, int FREQUENCYNO, int userid);
        Task<int> BDODATACOUNT(int FREQUENCYNO);
        string BDODataSubmission(XElement xml, int FREQUENCYNO,int FREQUENCYID, int userid, string prevmonth, string endDate);

        string NEWBDODataSubmission(int FREQUENCYNO,string prevstartdate, string prevenddate, XElement xml);
        string BDOTempDataSubmission(XElement xml, int STATUS, int userid);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllBDOData(int LevelId,int UserId);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllCollectorApproveData(int LevelId, int UserId);
        //COLLECTOR DATA
        Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetAllCollectorData(ABP.Domain.DataPoint.DataPoint bd);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllDepartmentDashboard(int LevelId,int sectorid,int frequency,int year);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> BDODataIdGetById(int SECTORID, string frequencyvalue, int Year);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> SearchBDOData(int LevelId, int SectorId, int Year, int FrequencyId, string FrequencyValue, string FromDate, string ToDate);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetBDODataById(int BDODataId);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAlldatapoints(int sectorid,int frequency, int Status);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllSearchbdodata(int sectorid, int frequency, int YEAR,int LeveDetailId,int UserId);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllSearchcollectorApprovedata(int sectorid, int frequency, int YEAR, int LeveDetailId, int UserId);
        
        Task<IEnumerable<ABP.Domain.Indicator.Indicator>> Getdatapoints(int sectorid, int frequency, int Status);
        Task<IEnumerable<IndicatorMapping>> GetDataPointID(int INDICATORID);
        string TrackOTP(ABP.Domain.Login.OTPTracker.OTPTracker OTP);
        string GetLastFreq(int LeveDetailId,int SectorId, int FreqId);
        Task<int> MONTHDATACOUNT(int FreqId,int FREQUENCYNO);
        Task<int> FRESHDATACOUNT(int FreqId, int FREQUENCYNO);
        Task<IEnumerable<ABP.Domain.CollectorData.CollectorDataAlert>> GetCollecterAlertData(int districid);
        Task<IEnumerable<ABP.Domain.CollectorData.CollectorDataAlert>> GetDeptAlertData(int districid);
    }
}
