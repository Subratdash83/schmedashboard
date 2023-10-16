using ABP.Domain;
using ABP.Repository.Contract.Contract.SMS;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.SMS
{
    public class SMSRepository : RepositoryBase, ISMSRepository
    {
        private readonly ILogger<SMSRepository> Logger;
        public SMSRepository(IConnectionFactory connectionFactory, ILogger<SMSRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("SMSRepository initialized");
        }
        public async Task<IEnumerable<ABP.Domain.SMS.SMS>> GetMobilenumber()
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "GET");
                p.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETMOBILENUMBER";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMS>(query, p, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        public List<ABP.Domain.SMS.SMS> GetUserDetails()
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "USERDETAILS");
                p.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETMOBILENUMBER";
                var result =  Connection.Query<ABP.Domain.SMS.SMS>(query, p, commandType: CommandType.StoredProcedure).AsList();
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Domain.SMS.SMS GetBDODATA(ABP.Domain.SMS.SMS sMS)
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "DataCount");
                p.Add("P_INTUSERID", OracleDbType.Int32, ParameterDirection.Input,sMS.intuserid);
                p.Add("P_FROMDATE", OracleDbType.Varchar2, ParameterDirection.Input, sMS.FROMDATE);
                p.Add("P_TODATE", OracleDbType.Varchar2, ParameterDirection.Input, sMS.TODATE);
                p.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETMOBILENUMBER";
                var result = Connection.QueryFirstOrDefault<ABP.Domain.SMS.SMS>(query, p, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void TrackSMS(int INTUSERID, string TEMPLATENAME, string smsresult)
        {
            try
            {
                string strOutput = "";
                IEnumerable<SuccessMessage> SuccessMessages;
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSERT");
                //p.Add(" P_SMSTRACKERID", OracleDbType.Int32, ParameterDirection.Input, SMSTRACKERID);
                p.Add("P_USERID", OracleDbType.Int32, ParameterDirection.Input, INTUSERID);
                p.Add("P_TEMPLATEDLTID", OracleDbType.Varchar2, ParameterDirection.Input, TEMPLATENAME);
                p.Add("P_SMSRESPONSE", OracleDbType.Varchar2, ParameterDirection.Input, smsresult);
                p.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETMOBILENUMBER";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
