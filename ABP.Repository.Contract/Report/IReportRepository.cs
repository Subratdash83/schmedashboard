using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Report
{
   public interface IReportRepository : IDisposable, IRepository
    {
        Task<IEnumerable<Domain.Report.Report>> ViewDistrict();
        Task<IEnumerable<Domain.Report.Report>> GetControlsByPanelid(int SectorId);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetDashboardData(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetMonthlyDashboardData(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetMonthlyYearlyDashboardData(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetMonthlyYearlyDashboardData2(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetYearlyWiseReportData(Domain.Report.Report Rpt);
       
        Task<IEnumerable<ABP.Domain.Report.Report>> GetIndicatorData(Domain.Report.Report Rpt);

        Task<IEnumerable<ABP.Domain.Report.Report>> GetIndicatorYearlyData(Domain.Report.Report Rpt);

        Task<IEnumerable<ABP.Domain.Report.Report>> GetDashboardDPData(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetMonthwiseReportData(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetReportDashboardData(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetAnnualWiseReportData(Domain.Report.Report Rpt);
        Task<IEnumerable<ABP.Domain.Report.Report>> GetLogReport(Domain.Report.Report Rpt);

        Task<IEnumerable<ABP.Domain.Report.MapData>> GetDistrictWiseReport(Domain.Report.Report Rpt);

        Task<IEnumerable<ABP.Domain.Report.MapData>> GetBlockByDistrictReport(Domain.Report.Report Rpt);

        Task<IEnumerable<ABP.Domain.Report.MapData>> GetMapDatas(int id,int year,int month);

        Task<IEnumerable<ABP.Domain.Report.MapData>> GetDistrictDatas(int id, int year, int month);
        Task<IEnumerable<ABP.Domain.BlockMap.BlockMap>> GetBlockMapData(int id);
        Task<ABP.Domain.Report.MapData> GetDistMapDatas(int id);

        Task<IEnumerable<ABP.Domain.Report.Report>> GetBlockMapDatas(int distid, int year, int freqno);

        DataSet GetLogData(string APPLICATIONNO, string PanelName,int SectorId);

        Task<ABP.Domain.Report.Report> GetApplicationNO(int blockid, int year, int freqno,int freqid);
        Task<ABP.Domain.Report.Report> GetApplicationNOyearly(int blockid, int year,int freqid);
        Task<IEnumerable<ABP.Domain.Report.MapData>> GetMapDatascore(int id, string year, int month);
        Task<IEnumerable<ABP.Domain.Report.BlockDpData>> GetBlockDatapoint(int blockid, string Appno);
        Task<IEnumerable<ABP.Domain.Report.AbstractData>> GetDashboarAbstractdData(Domain.Report.AbstractData Rpt);
        Task<IEnumerable<ABP.Domain.Report.SectorPRFData>> GetSectorEntryPrfData(Domain.Report.SectorPRFData Rpt);
        Task<IEnumerable<ABP.Domain.Report.SectorPRFData>> GetSectorEntryTotalPrfData(Domain.Report.SectorPRFData Rpt);
        Task<IEnumerable<ABP.Domain.Report.SectorPRFData>> GetSectorEntryYearTotal(Domain.Report.SectorPRFData Rpt);
        Task<ABP.Domain.Report.Report> ApplicationNOyearly(int blockid, int year, int freqid,int monthno);
        string GetDpcount();
        Task<IEnumerable<ABP.Domain.Report.MapData>> GetDistrictwiseMapData(int year, int freqno);
        Task<IEnumerable<ABP.Domain.Report.MapData>> GetEnterednotenteredData(int year, int freqno);
        Task<IEnumerable<ABP.Domain.Report.MapData>> GetEnterednotenteredDatadistwise(int year, int freqno,int distid);
    }
}
