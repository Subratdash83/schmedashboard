using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.Panel
{
    public interface IControlPanelRepository 
    {
        string InsertControlPanel(ABP.Domain.Panel.ControlPanel ControlPanel);

        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> ViewControlPanel(int SECTORID,int frequency);

        string DeleteControlPanel(ABP.Domain.Panel.ControlPanel ControlPanel);

        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> ControlPanelGateById(int id);
        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> ControlPanelGateByPanelId(int id,int fid);
        string AlterTable(string TableName,string ControlName);

        string InsertControlExpression(ABP.Domain.DataPointExpression.DatapointExpression DPEx);
        string ModifySLNO(int primaryId, int slno);
        Task<IEnumerable<ABP.Domain.DataPointExpression.DatapointExpression>> GetAllExpressionData();
        Task<IEnumerable<ABP.Domain.DataPointExpression.DatapointExpression>> GetExpressionDataById(int ExpressionId,int SectorId);
        string DeleteExpression(ABP.Domain.DataPointExpression.DatapointExpression Expression);
        Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> CommonControlPanelData();
    }
}
