using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.DistrictData
{
    public interface IDistrictDataRepository
    {
      
        string AcceptPendingIndicatorData(ABP.Domain.BlockData.BlockData bd);
        string RejectPendingIndicatorData(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<Domain.Indicator.Indicator>> GetIndicatorsWithFormula(ABP.Domain.BlockData.BlockData bd);

        string CheckDataPointYearly(int Datapointid);
        Task<IEnumerable<Domain.Indicator.Indicator>> GetIndicatorsWithFormulaByFreq(ABP.Domain.BlockData.BlockData bd);
        List<Domain.Panel.ControlPanel> GetAllDPByIndicator(Domain.Indicator.Indicator ind);
        Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetAllCollectorData(ABP.Domain.DataPoint.DataPoint bd);
        Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetAllBlocksApprovedData(ABP.Domain.DataPoint.DataPoint bd,string Action); 
        Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetAllDepartmentData(ABP.Domain.BlockData.BlockData bd);
        string RejectASectorData(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<Domain.Indicator.Indicator>> GetAllIndicatorsForApproval(ABP.Domain.BlockData.BlockData bd);
        Task<IEnumerable<Domain.Indicator.Indicator>> Viewindicator();
        Task<IEnumerable<Domain.Block.Block>> GetBlockByDist(int DistCode);
        Task<IEnumerable<Domain.Block.Block>> GetDistrict(int DistCode);
        Task<IEnumerable<ABP.Domain.Department.DistrictData>> GetAllDistrict(int DistCode);
        string GetRejectSectorCount(string ApplicNo);
    }
}
