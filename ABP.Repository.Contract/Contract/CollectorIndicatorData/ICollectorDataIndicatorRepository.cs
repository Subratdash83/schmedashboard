using ABP.Domain.CollectorData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Repository.Contract.Contract.CollectorIndicatorData
{
    /// <summary>
    /// Interface ICollectorIndicatorDataRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface ICollectorDataIndicatorRepository
    {
        /// <summary>
        /// Insert CollectorData Detail
        /// </summary>
        /// <param name="ManageCollectorData">CollectorIndicatorData ParaMeter</param>
        /// <returns><seealso cref="Domain.CollectorData.CollectorData"/></returns>
        
        //string AddCollectorDataIndicatorTemp(Domain.Indicator.Indicator indicator);
        //string AddCollectorDataIndicatorTemp(XElement xml, int Indicatorid, string FromDate, string ToDate, string FrequencyValue, int Blockid, int STATUS, int userid);
        string AddCollectorDataIndicatorTemp(XElement xml, int STATUS, int userid,string Remark);
        //string AddCollectorDataIndicator(Domain.Indicator.Indicator indicator);
        //string AddCollectorDataIndicator(XElement xml, int Indicatorid, string FromDate, string ToDate, string FrequencyValue, int Blockid, int STATUS, int userid);
        string AddCollectorDataIndicator(XElement xml, XElement xml1, int STATUS, int userid, string Remark);

        string AddCollectorDataIndicatorInitalValue(Domain.Indicator.IndicatorMapping indicator);
        Task<IEnumerable<Domain.Indicator.Indicator>> GetAllCollectorIndicator(int LevelId);

        Task<IEnumerable<Domain.Indicator.Indicator>> GetAllSearchCollectordata(int IndicatorId, int frequency, int YEAR, int LevelId);

        Task<IEnumerable<Domain.Indicator.Indicator>> SearchCollectorIndicator(int LevelId, int IndicatorId, int Year, int FrequencyId, string FrequencyValue, string FromDate, string ToDate);
        Task<IEnumerable<Domain.CollectorData.CompositeScore>> GetCompositeScore(CompositeScore composite);
    }
}
