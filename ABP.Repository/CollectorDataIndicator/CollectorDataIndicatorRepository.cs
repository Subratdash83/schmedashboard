using ABP.Domain;
using ABP.Domain.CollectorData;
using ABP.Repository.Contract.Contract.CollectorIndicatorData;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ABP.Repository.CollectorDataIndicator
{
    public class CollectorDataIndicatorRepository: RepositoryBase, ICollectorDataIndicatorRepository
    {
        private readonly ILogger<CollectorDataIndicatorRepository> Logger;
        public CollectorDataIndicatorRepository(IConnectionFactory connectionFactory, ILogger<CollectorDataIndicatorRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("CollectorDataIndicatorRepository initialized");
        }
        public string AddCollectorDataIndicatorInitalValue(Domain.Indicator.IndicatorMapping indicator)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, indicator.INDICATORID);
                dyParam.Add("P_DDataPointId", OracleDbType.Int32, ParameterDirection.Input, indicator.DDATAPOINTID); 
                dyParam.Add("P_NDataPointId", OracleDbType.Int32, ParameterDirection.Input, indicator.NDATAPOINTID);
                //dyParam.Add("P_LevelDetailId", OracleDbType.Int32, ParameterDirection.Input, indicator.BLOCKID);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, indicator.CREATEDBY);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORINDINITIAL";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, indicator);
                throw ex;
            }
            //return strOutput;
        }


        //Updated new xml  repositary..
        public string AddCollectorDataIndicator(XElement xml, XElement xml1, int STATUS, int userid,string Remark)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, STATUS);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, userid);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_XML_DATA1", OracleDbType.XmlType, ParameterDirection.Input, xml1);
                dyParam.Add("P_Remark", OracleDbType.Varchar2, ParameterDirection.Input, Remark);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORINDADDUPDATE";
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

      
        //Updated new temp xml repositary..

        public string AddCollectorDataIndicatorTemp(XElement xml, int STATUS ,int userid,string Remark)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, STATUS);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, userid);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_Remark", OracleDbType.Varchar2, ParameterDirection.Input, Remark); 
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORINDTEMP";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, xml);
                throw ex;
            }
            //return strOutput;
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> GetAllCollectorIndicator(int LevelId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_LevelId", OracleDbType.Varchar2, ParameterDirection.Input, LevelId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORINDVIEW";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Indicator.Indicator>> GetAllSearchCollectordata(int IndicatorId, int frequency, int YEAR, int LevelId)
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
               // dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "A");
                dyParam.Add("P_INDICATORID", OracleDbType.Int32, ParameterDirection.Input, IndicatorId);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, frequency);
                dyParam.Add("P_LevelId", OracleDbType.Int32, ParameterDirection.Input, LevelId);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORINDVIEW";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }




        }















        public async Task<IEnumerable<Domain.Indicator.Indicator>> SearchCollectorIndicator(int LevelId, int IndicatorId, int Year, int FrequencyId, string FrequencyValue, string FromDate, string ToDate)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_LevelId", OracleDbType.Varchar2, ParameterDirection.Input, LevelId);
                dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, IndicatorId);
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, FrequencyId);
                dyParam.Add("P_FrequencyValue", OracleDbType.Varchar2, ParameterDirection.Input, FrequencyValue);
                dyParam.Add("P_FromDate", OracleDbType.Varchar2, ParameterDirection.Input, FromDate);
                dyParam.Add("P_ToDate", OracleDbType.Varchar2, ParameterDirection.Input, ToDate);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORINDSEARCH";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.CollectorData.CompositeScore>> GetCompositeScore(CompositeScore composite)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_DistId", OracleDbType.Int32, ParameterDirection.Input, composite.DISTRICT_CODE);
                dyParam.Add("P_BlockId", OracleDbType.Int32, ParameterDirection.Input, composite.BLOCK_CODE);
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, composite.SECTORID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COMPOSITESCOREVIEW";
                var result = await Connection.QueryAsync<Domain.CollectorData.CompositeScore>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
