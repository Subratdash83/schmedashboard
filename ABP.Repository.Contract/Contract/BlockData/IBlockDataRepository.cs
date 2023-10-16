using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.BlockData
{
    public interface IBlockDataRepository
    {
        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByFrequency(ABP.Domain.Panel.ControlPanel cp);
        //Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetYearlyAppNo(ABP.Domain.Panel.ControlPanel cp);
        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByFrequencyMY(ABP.Domain.Panel.ControlPanel cp);
        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByFrequencyearly(ABP.Domain.Panel.ControlPanel cp);
        string GetLastFreq(int LeveDetailId, int SectorId, int FreqId);
        string GetYearlyAppNo(int BLOCKID, int DPMONTH, int YEAR);
        
        Task<int> FRESHDATACOUNT(int FreqId, int FREQUENCYNO, int BlockId,int Year);
        Task<int> MONTHDATACOUNT(int FreqId, int FREQUENCYNO, int BlockId,int Year);
        Task<int> FREQDATACOUNT(int FreqId);
        string SubmitBlockData(List<StringBuilder> sb);
        string AddBlockDataMain(ABP.Domain.BlockData.BlockData bd);
        string AddBlockDataMainTmp(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockData(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockDataTmp(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockDataByAppNo(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockDatas(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetAllBlockData(ABP.Domain.BlockData.BlockData bd);

        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetAllBlockDataForAutoApproval();
        Task<IEnumerable<Domain.Frequency.FrequencyDP>> ViewFrequency();
        decimal? GetContolValue(ABP.Domain.Panel.ControlPanel cp, string AppNo);
        decimal? GetContolValuecol(ABP.Domain.Panel.ControlPanel cp, string AppNo,int BLOCKID,int YEAR, int FREQUENCYID, int FREQUENCYNO);
        decimal? GetContolValueyear(ABP.Domain.Panel.ControlPanel cp, string AppNo, int BLOCKID, int YEAR, int FREQUENCYID, int FREQUENCYNO);
        decimal? GetContolValuecolyear(ABP.Domain.Panel.ControlPanel cp, string AppNo, int BLOCKID, int YEAR, int FREQUENCYID, int FREQUENCYNO);
        decimal GetCountValue(string PANELNAME, string AppNo);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> Getdatapoints(int sectorid, int frequency, int Status);
        string UpdateBlockDataMain(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetRejectedControls(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetFromDateToDate(int FreqId, int MONTH, int BlockId);
        Task<IEnumerable<ABP.Domain.Frequency.FrequencyDP>> GetFrequencyValue(ABP.Domain.BlockData.BlockData data);

        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> ViewFreezedetails(int DISTRICTID, int BLOCKID);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetMonthlyFreezeDashboardData(ABP.Domain.BlockData.BlockData data);

        string UpdateFreezeDashboardData(string ApplicationNo, int Blockid, int Districtid, int Status, int year,int frequencyid, int frequencyno, int Userid);
        Task<IEnumerable<ABP.Domain.Frequency.FrequencyDP>> GetFrequencyValuemoth(int frequencyid);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetEnteredDatapoints(string AppNo, string PanelName);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetAllControlPanelDP(string PanelName, int FreqId, int FreqNo);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> DATA();
        string BlockDatapointYearlyCheck(int Year, int Blockid, int Frequencyid);
        Task<IEnumerable<ABP.Domain.BlockData.BlockDataAlert>> GetBDOAlertData(int blockid);
        Task<IEnumerable<ABP.Domain.BlockData.BlockDataAlert>> GetBDOAlertDataPoint(int blockid);
        Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockEnteredDataByYear(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByIndicators(ABP.Domain.Panel.ControlPanel cp);
        // GetCollectorAllert
        string UpdateRejData(string Applino);
        string GetRejectAppno(int Year, int FREQUENCYNO, int Blokid, int FrequencyId);
        Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> ViewRejectedBlockDetails(int BLOCKID);
    }
}
