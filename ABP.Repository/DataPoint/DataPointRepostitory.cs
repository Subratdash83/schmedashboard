using ABP.Domain;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Repository.DataPoint
{
    public class DataPointRepostitory : RepositoryBase, IDataPointRepository
    {
        private readonly ILogger<DataPointRepostitory> Logger;
        public DataPointRepostitory(IConnectionFactory connectionFactory, ILogger<DataPointRepostitory> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("DataPointRepostitory initialized");
        }
        public string AddDataPoint(Domain.DataPoint.DataPoint dp)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (dp.DATAPOINTID != 0)
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                    dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, dp.SECTORID);
                    dyParam.Add("P_DataPointId", OracleDbType.Int32, ParameterDirection.Input, dp.DATAPOINTID);
                    dyParam.Add("P_DataPointName", OracleDbType.Varchar2, ParameterDirection.Input, dp.DATAPOINTNAME);
                    dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, dp.FREQUENCYID);
                    dyParam.Add("P_DataPointDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.DATAPOINTDATE);
                    dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, dp.CREATEDBY);
                    dyParam.Add("P_iscensus", OracleDbType.Int32, ParameterDirection.Input, dp.iscensus);
                    dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                    var query = "USP_ABP_DATAPOINTADDUPDATE";
                    SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                    strOutput = SuccessMessages.AsList()[0].successid.ToString();
                }
                else
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "INSERT");
                    dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, dp.SECTORID);
                    dyParam.Add("P_DataPointId", OracleDbType.Int32, ParameterDirection.Input, dp.DATAPOINTID);
                    dyParam.Add("P_DataPointName", OracleDbType.Varchar2, ParameterDirection.Input, dp.DATAPOINTNAME);
                    dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, dp.FREQUENCYID);
                    dyParam.Add("P_DataPointDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.DATAPOINTDATE);
                    dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, dp.CREATEDBY);
                    dyParam.Add("P_iscensus", OracleDbType.Int32, ParameterDirection.Input, dp.iscensus);
                    dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                    var query = "USP_ABP_DATAPOINTADDUPDATE";
                    SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                    strOutput = SuccessMessages.AsList()[0].successid.ToString();
                }
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, dp);
                throw ex;
            }
            //return strOutput;
        }

        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> ViewDataPoint(int sectorid,int datapointid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_Sectorid", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_DataPointId", OracleDbType.Int32, ParameterDirection.Input, datapointid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DATAPOINTVIEWDELETE";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string DeleteDataPoint(Domain.DataPoint.DataPoint Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_DataPointId", OracleDbType.Int32, ParameterDirection.Input, Model.DATAPOINTID);
                p.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DATAPOINTVIEWDELETE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> DataPointGateById(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_DataPointId", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETDATAPOINTBYID";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Frequency.FrequencyDP>> ViewFrequency()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_FREQUENCYVIEWDELETE";
                var result = await Connection.QueryAsync<Domain.Frequency.FrequencyDP>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Frequency.FrequencyDP>> ViewFrequencyWiseMonth()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEWM");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_FREQUENCYVIEWDELETE";
                var result = await Connection.QueryAsync<Domain.Frequency.FrequencyDP>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> DataPointGateBySectorId(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETDATAPOINTBYSECID";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> DataPointiscensus(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_IS_CENSUS";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetDataPointBySectorAndFrequency(Domain.DataPoint.DataPoint dataPoint)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, dataPoint.SECTORID);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, dataPoint.FREQUENCYID);
                var query = "USP_ABP_DPBYSECTORANDFREQ";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> DPGroupBySector(int Status, int LevelId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_LevelId", OracleDbType.Varchar2, ParameterDirection.Input, LevelId);
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, Status);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DATAPOINTSBYSECTOR";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> CollectorDPGPBySector(int Status, int LevelId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_LevelId", OracleDbType.Varchar2, ParameterDirection.Input, LevelId);
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, Status);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORDPSBYSECTOR";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> DPAndValueByDpId(int Status, int LevelId, int DPId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_LevelId", OracleDbType.Int32, ParameterDirection.Input, LevelId);
                //dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, Status);
                dyParam.Add("P_DPId", OracleDbType.Int32, ParameterDirection.Input, DPId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LASTDPANDVALUEBYDPID";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Datadashboard(XElement xml, int DistID,int code)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "ADD");
                dyParam.Add("P_DistID", OracleDbType.Int32, ParameterDirection.Input, DistID);
                dyParam.Add("P_DISTCODE", OracleDbType.Int32, ParameterDirection.Input, code);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DASHBOARDDATA";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, xml);
                throw ex;
            }
        }
        public string IndicatorDataInsertAutoApp(XElement xml) 
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDAUTOAPP";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, xml);
                throw ex;
            }
        }
    }
}
