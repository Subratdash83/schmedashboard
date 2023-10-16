using ABP.Domain;
using ABP.Repository.Contract.Contract.SMSTemplate;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.SMSTemplate
{
   public class SMSTemplateRepository : RepositoryBase, ISMSTemplateRepository
    {
        private readonly ILogger<SMSTemplateRepository> Logger;
        public SMSTemplateRepository(IConnectionFactory connectionFactory, ILogger<SMSTemplateRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("SMSRepository initialized");
        }

        public List<Domain.SMSTemplate.SMSTemplate> GetTemplatesByDay(ABP.Domain.SMSTemplate.SMSTemplate sMS)
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "SELECTALL");
                //p.Add("P_TEMPLATEID", OracleDbType.Int32, ParameterDirection.Input, sMS.TEMPLATEID);
                //p.Add("P_SMSFOR", OracleDbType.Int32, ParameterDirection.Input, sMS.SMSFOR);
                //p.Add("P_SMSCONTENT", OracleDbType.Int32, ParameterDirection.Input, sMS.SMSCONTENT);
                //p.Add("P_DAY", OracleDbType.Int32, ParameterDirection.Input,sMS.DAY);
                //p.Add("P_TIME", OracleDbType.Int32, ParameterDirection.Input, sMS.TIME);
                //p.Add("P_TIMEPERIOD", OracleDbType.Int32, ParameterDirection.Input, sMS.TIMEPERIOD);
                p.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_INFO";
                var result = Connection.Query<Domain.SMSTemplate.SMSTemplate>(query, p, commandType: CommandType.StoredProcedure).AsList();
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.SMSTemplate.SMSTemplate>> GetTimePeriod()
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "GET");
                p.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_INFO";
                var result = await Connection.QueryAsync<ABP.Domain.SMSTemplate.SMSTemplate>(query, p, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string InsertSMSTemplate(ABP.Domain.SMS.SMSData sms)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSERT");
                dyParam.Add("P_EVENTID", OracleDbType.Int32, ParameterDirection.Input, sms.EVENTID);
                dyParam.Add("P_TEMPLATEID", OracleDbType.Int32, ParameterDirection.Input, sms.TEMPLATEID);
                dyParam.Add("P_TEMPLATETYPE", OracleDbType.Int32, ParameterDirection.Input, sms.TEMPLATETYPE);
                dyParam.Add("P_JOBTITLE", OracleDbType.Varchar2, ParameterDirection.Input, sms.JOBTITLE);
                dyParam.Add("P_USERTYPE", OracleDbType.Varchar2, ParameterDirection.Input, sms.USERTYPE);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, sms.DISTRICTID);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, sms.BLOCKID);
                dyParam.Add("P_DATE", OracleDbType.Date, ParameterDirection.Input, sms.TEMPDATE);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string UpdateSMSTemplate(ABP.Domain.SMS.SMSData sms)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                dyParam.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input, sms.ID);
                dyParam.Add("P_EVENTID", OracleDbType.Int32, ParameterDirection.Input, sms.EVENTID);
                dyParam.Add("P_TEMPLATEID", OracleDbType.Int32, ParameterDirection.Input, sms.TEMPLATEID);
                dyParam.Add("P_TEMPLATETYPE", OracleDbType.Int32, ParameterDirection.Input, sms.TEMPLATETYPE);
                dyParam.Add("P_JOBTITLE", OracleDbType.Varchar2, ParameterDirection.Input, sms.JOBTITLE);
                dyParam.Add("P_USERTYPE", OracleDbType.Varchar2, ParameterDirection.Input, sms.USERTYPE);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, sms.DISTRICTID);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, sms.BLOCKID);
                dyParam.Add("P_DATE", OracleDbType.Date, ParameterDirection.Input, sms.TEMPDATE);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewSMSTemplates()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewTemplate()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VWTEMP");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewAllSmsTemplate(DateTime smsdate)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEWSTS");
                dyParam.Add("P_DATE", OracleDbType.Date, ParameterDirection.Input, smsdate);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_SEND";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetEventDetails()
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "EVENT");
                p.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, p, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {

               throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetTemplateDetails()
        {
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VWTEMP");
                p.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, p, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string InsertSMSTemplateDetail(ABP.Domain.SMS.SMSData sms)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSTEMP");
                dyParam.Add("P_TEMPLATECODE", OracleDbType.Varchar2, ParameterDirection.Input, sms.TEMPLATECODE);
                dyParam.Add("P_TEMPLATETITLE", OracleDbType.Varchar2, ParameterDirection.Input, sms.TEMPLATETITLE);
                dyParam.Add("P_TEMPLATEMESSAGE", OracleDbType.Varchar2, ParameterDirection.Input, sms.TEMPLATEMESSAGE);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertSendSMSDetail(int DISTRICTID,int BLOCKID,string MOBNO,string Substs)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSTSMSDET");
                dyParam.Add("P_DISTRICTID", OracleDbType.Varchar2, ParameterDirection.Input, DISTRICTID);
                dyParam.Add("P_BLOCKID", OracleDbType.Varchar2, ParameterDirection.Input, BLOCKID);
                dyParam.Add("P_MOBNO", OracleDbType.Varchar2, ParameterDirection.Input, MOBNO);
                dyParam.Add("P_TEMPLATEMESSAGE", OracleDbType.Varchar2, ParameterDirection.Input, Substs);               
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetUserDetailsForSMS(ABP.Domain.SMS.SMSData sms)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "USERDTLS");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, sms.DISTRICTID);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, sms.BLOCKID);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetAllUserDetailsForSMS()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SMSDETAIL");
               
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_CONFIG";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> PostSendSMS()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SEND");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_SEND";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> PostSendSMSCol(int  distid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "VIEWCOL");
                dyParam.Add("P_DISTID", OracleDbType.Int32, ParameterDirection.Input, distid);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_SEND";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> PostSMScol()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "COLDET");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_SEND";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertJobRecord(int distid, int freqid, int status, string jobtype)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessage;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSERT");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, distid);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, freqid);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, status);
                dyParam.Add("P_JOBTYPE", OracleDbType.Varchar2, ParameterDirection.Input, jobtype);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_AUTO_JOBPROCESS";
                //var result =Connection.Query<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                SuccessMessage = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure) ;
                strOutput = SuccessMessage.AsList()[0].successid.ToString();
                //return Convert.ToInt32(result);
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateJobRecord(int status,int Jobid)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessage;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, status);
                dyParam.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input, Jobid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_AUTO_JOBPROCESS";
                SuccessMessage = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessage.AsList()[0].successid.ToString();
                //return Convert.ToInt32(result);
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertIndicatorJobDetails(ABP.Domain.SMS.SMSData sms)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "INSERTIJOB");
                dyParam.Add("P_JOBTYPEID", OracleDbType.Int32, ParameterDirection.Input, sms.JOBTYPEID);
                dyParam.Add("P_INDJOBTYPEID", OracleDbType.Int32, ParameterDirection.Input, sms.INDJOBTYPEID);
                dyParam.Add("P_JOBDATE", OracleDbType.Date, ParameterDirection.Input, sms.JOBDATE);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_AUTO_JOBPROCESS";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewIndicatorJobDetails()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEWIJOB");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_AUTO_JOBPROCESS";
                var result = await Connection.QueryAsync<ABP.Domain.SMS.SMSData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteIndicatorJobDetails(int ID)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input, ID);
                p.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_AUTO_JOBPROCESS";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
