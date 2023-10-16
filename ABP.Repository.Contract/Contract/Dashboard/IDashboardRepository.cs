using ABP.Domain.Dashboard;
using ABP.Domain.DashboardModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.Dashboard
{
   public interface IDashboardRepository : IDisposable, IRepository
    {
        Task<IEnumerable<DashboardModel>> GetDashboardData(int Distid, int Blockid);
        Task<IEnumerable<DashboardModel>> GetDashboardDetails(int SectorId);
        Task<IEnumerable<DashboardModel>> GetCompositeData();
        Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetRejectedDataDetails(int Leveldetailid, int Status);
        Task<IEnumerable<DashboardNew>> AllSectorIndicatorcounts();
        Task<IEnumerable<DashboardNew>> AllSectorDatapointcounts(int month);
        Task<IEnumerable<DashboardNew>> MonthlyDatapointcounts(int year, int month, int blockid);
        Task<IEnumerable<DashboardNew>> YearlyDatapointcounts(int year, int month, int blockid);
        Task<IEnumerable<DashboardNew>> GetIndicatorBySector(int sectorid);
        Task<IEnumerable<DashboardNew>> GetBlockWiseDataByDistID(int year, int month, int distid);
        Task<IEnumerable<DashboardNew>> GetSectorDataByBlockID(int year, int month, int blockid);
        Task<IEnumerable<DashboardNew>> GetIndicatorDPBySector(int year, int month, int blockid, int sectorid);
    }
}
