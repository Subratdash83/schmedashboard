using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ABP.Domain.Indicator;
using ABP.Domain.Sector;

namespace ABP.Repository.Contract.Contract.Indicator
{
    /// <summary>
    /// Interface IIndicatorRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface IIndicatorRepository
    {
        /// <summary>
        /// Get Indicator Detail
        /// </summary>
        /// <param name="IndicatorClosure">Indecator Request ParaMeter</param>
        /// <returns><seealso cref="Domain.Indicator.Indicator"/></returns>
        /// 
        string InsertIndicator(Domain.Indicator.Indicator indicator);
        string AddUpdateIndicatorsFormula(Domain.Indicator.Indicator indicator);

        Task<IEnumerable<Domain.Indicator.Indicator>> ViewIndicator(int sectorid,int indicatorid, int frequencyid);

        string DeleteIndicator(Domain.Indicator.Indicator indicator);

        Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorGateById(int id);

        string AddIndicatorMapping(Domain.Indicator.IndicatorMapping indicatormapping);

        Task<IEnumerable<Domain.Indicator.IndicatorMapping>> ViewIndicatorMapping(string ACTION,int sectorid,int indicatorid);
        Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorGateBySectorId(int id, int fid);
        string DeleteIndicatorMapping(Domain.Indicator.IndicatorMapping indicatormapping);
        Task<IEnumerable<Domain.Indicator.IndicatorMapping>> IndicatorMappingGateByIMId(int id);
        Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorsGPBySector(int Frequency, int BlockId);
        Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorsByBlockIdGPBySector(int BlockId);
        Task<IEnumerable<Domain.Indicator.IndicatorMapping>> IndicatorMappingGateBySectorAndIMId(int SectorId, int IMId);
        Task<IEnumerable<Domain.Indicator.Indicator>> GETALLIndicators(int Frequency);
        List<Domain.Indicator.Indicator> GETALLDP(string applicationNo, int Frequency);
        decimal GetContolValueThroughQuery(string Query);
        decimal GetContolValueThroughQueryindscore(string Query, int blockid, int year,int frequencyid ,int FREQUENCYNO);
        decimal GetContolValueThroughQueryindYearly(string Query, int blockid, int year, int frequencyid, int FREQUENCYNO);
        
        string GetAppValueThroughQuery(string Query);
        string GetContolThroughQuery(string Query);
        Task<IEnumerable<Domain.Indicator.IndicatorValue>> IndicatorValues();
    }
}
