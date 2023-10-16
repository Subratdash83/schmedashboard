using ABP.Domain;
using ABP.Repository.Contract.Contract.CollectorData;
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

namespace ABP.Repository.CollectorData
{
    public class CollectorDataRepository: RepositoryBase, ICollectorDataRepository
    {
        private readonly ILogger<CollectorDataRepository> Logger;
        public CollectorDataRepository(IConnectionFactory connectionFactory, ILogger<CollectorDataRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("CollectorDataRepository initialized");
        }
        public void AddCollectorData(Domain.DataPoint.DataPoint dp)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_BDODataId", OracleDbType.Int32, ParameterDirection.Input, dp.BDODATAID);
                dyParam.Add("P_Value", OracleDbType.Decimal, ParameterDirection.Input, dp.DATAPOINTVALUE);
                dyParam.Add("P_Remark", OracleDbType.Varchar2, ParameterDirection.Input, dp.COLLECTORREMARK);
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, dp.STATUS);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, dp.CREATEDBY);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORDATAAPPROVAL";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                //return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, dp);
                throw ex;
            }
            //return strOutput;
        }


        public string AddCollectorData(XElement xml, int STATUS, int userid,string Remark)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
               
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, STATUS);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, userid);
                dyParam.Add("P_Remark", OracleDbType.Varchar2, ParameterDirection.Input, Remark);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORDATAAPPROVAL";
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
    }
}
