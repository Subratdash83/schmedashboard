using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Repository.Contract.Contract.DataPoint
{

    /// <summary>
    /// Interface IIndicatorRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface IDataPointRepository
    {
        /// <summary>
        /// Get DataPoint Detail
        /// </summary>
        /// <param name="ManageDataPoint">DataPoint ParaMeter</param>
        /// <returns><seealso cref="Domain.DataPoint.DataPoint"/></returns>

        string AddDataPoint(Domain.DataPoint.DataPoint dp);

        string Datadashboard(XElement xEle, int DistID,int code);
        string IndicatorDataInsertAutoApp(XElement xml);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> ViewDataPoint(int sectorid,int datapointid);

        string DeleteDataPoint(Domain.DataPoint.DataPoint dp);

        Task<IEnumerable<Domain.DataPoint.DataPoint>> DataPointGateById(int id);
        Task<IEnumerable<Domain.Frequency.FrequencyDP>> ViewFrequency();
        Task<IEnumerable<Domain.Frequency.FrequencyDP>> ViewFrequencyWiseMonth();
        Task<IEnumerable<Domain.DataPoint.DataPoint>> DataPointGateBySectorId(int id);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> DataPointiscensus(int id);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> GetDataPointBySectorAndFrequency(Domain.DataPoint.DataPoint dataPoint);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> DPGroupBySector(int Status, int LevelId);
        Task<IEnumerable<Domain.DataPoint.DataPoint>> DPAndValueByDpId(int Status, int LevelId,int DPId);
    }
}
