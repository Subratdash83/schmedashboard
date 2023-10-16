using ABP.Domain;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.DistrictData
{
    public class DistrictDataRepository : RepositoryBase, IDistrictDataRepository
    {
        public IConfiguration Configuration { get; }
        public DistrictDataRepository(IConnectionFactory connectionFactory, IConfiguration configuration) : base(connectionFactory)
        {
            Configuration = configuration;
        }
        public string AcceptPendingIndicatorData(ABP.Domain.BlockData.BlockData bd)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.  Input, "ACCEPT");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_REMARK", OracleDbType.Varchar2, ParameterDirection.Input, bd.REMARK);
                dyParam.Add("P_CREATEDBY", OracleDbType.Int32, ParameterDirection.Input, bd.CREATEDBY);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetAllCollectorData(ABP.Domain.DataPoint.DataPoint bd)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VW");
                dyParam.Add("P_LeveldetailId", OracleDbType.Int32, ParameterDirection.Input, bd.leveldetailedid);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetAllBlocksApprovedData(ABP.Domain.DataPoint.DataPoint bd,string Action)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, Action);
                dyParam.Add("P_DISTID", OracleDbType.Int32, ParameterDirection.Input, bd.leveldetailedid);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, bd.SECTORID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetAllDepartmentData(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "DP");
                dyParam.Add("P_DISTID", OracleDbType.Int32, ParameterDirection.Input, bd.DISTRICTID);
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, bd.PANELID);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string RejectPendingIndicatorData(ABP.Domain.BlockData.BlockData bd)  
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "REJECT");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_REMARK", OracleDbType.Varchar2, ParameterDirection.Input, bd.REMARK);
                dyParam.Add("P_CREATEDBY", OracleDbType.Int32, ParameterDirection.Input, bd.CREATEDBY);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> GetIndicatorsWithFormula(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INDS");
                dyParam.Add("P_FrequencyId", OracleDbType.Varchar2, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CheckDataPointYearly( int Datapointid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "CD");
                dyParam.Add("P_CONTROLID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Datapointid));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORSCORE";
                var result = Connection.QueryFirstOrDefault<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return  result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> GetIndicatorsWithFormulaByFreq(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INDS");
                dyParam.Add("P_FrequencyId", OracleDbType.Varchar2, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DATAPOINT_VIEWS";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Domain.Panel.ControlPanel> GetAllDPByIndicator(Domain.Indicator.Indicator ind)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DPS");
                dyParam.Add("P_INDICATORID", OracleDbType.Int32, ParameterDirection.Input, ind.INDICATORID);
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, ind.SECTORID);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, ind.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                var result = Connection.Query<Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure).AsList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string RejectASectorData(ABP.Domain.BlockData.BlockData bd)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SIREJECT");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, bd.PANELID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_REMARK", OracleDbType.Varchar2, ParameterDirection.Input, bd.REMARK);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> GetAllIndicatorsForApproval(ABP.Domain.BlockData.BlockData bd) 
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INDSS");
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<Domain.Indicator.Indicator>> Viewindicator()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
               dyParam.Add("P_out_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATOR_VALUE";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetBlockByDist(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GB");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> GetDistrict(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "DIST");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Department.DistrictData>> GetAllDistrict(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "DSBK");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<ABP.Domain.Department.DistrictData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetRejectSectorCount(string ApplicNo)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "REJCNT");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, ApplicNo);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                var result = Connection.QueryFirstOrDefault<string>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
