using ABP.Domain.Sector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.Panel
{
    public interface IPanelRepository
    {
        string InsertPanel(ABP.Domain.Panel.Panel panel);

        Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewPanel();
        Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewPanel1();

        string DeletePanel(ABP.Domain.Panel.Panel panel);

        Task<IEnumerable<ABP.Domain.Panel.Panel>> PanelGateById(int id);
        string GetSeqValue(string SequenceName);
        string CreateTable(string TableName);

        Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewSector();

        Task<IEnumerable<Domain.Indicator.Indicator>> BindFormulaById(int id);
        Task<IEnumerable<ABP.Domain.Indicator.Indicator>> ViewIndicatorFormula();

        string DeleteIndicatorFormulaId(ABP.Domain.Indicator.Indicator Indicator);

        Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewLevels();

        Task<IEnumerable<ABP.Domain.Panel.Panel>> ViewProviders();
        Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorBySector(int sectorid);

        Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorBySectorYearly(int sectorid);

        Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorBySectorMonthly(int sectorid);

        Task<IEnumerable<ABP.Domain.Panel.Panel>> GetDatapointBySector(int sectorid);
        Task<IEnumerable<ABP.Domain.Panel.Panel>> GetIndicatorName(int sectorid, int indicatorid);
        string GetIndicatorTrend(int Distid, int indicatorid, int sectorid, int blockid, int Year);

        string GetIndicatorTrendv2(int Distid, int indicatorid, int sectorid, int blockid, int Year);
        string GetDataPointTrend(int Blockid, int sectorid, int indicatorid, int Year, string controlname);

        string GetDataPointTrendv2(string controlname, int sectorid, int indicatorid, int Year, string Blockid);
        string GetDataPointName(int sectorid, string controlname);

        string GetIndictorNameTrend(int sectorid, string indicatorid);

        string GetBlockNameTrend(int sectorid, string indicatorid);



    }
}
