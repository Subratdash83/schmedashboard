using ABP.Domain;
using ABP.Domain.Indicator;
using ABP.Repository.Contract.Contract.BDOData;
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

namespace ABP.Repository.BDOData
{
    public class BDODataRepository : RepositoryBase, IBDODataRepository
    {
        private readonly ILogger<BDODataRepository> Logger;
        public BDODataRepository(IConnectionFactory connectionFactory, ILogger<BDODataRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("BDODataRepository initialized");
        }
        public string UpdateBDOData(XElement xml, XElement xml2, Domain.DataPoint.DataPoint dp, int BLOCKID, int FREQUENCYNO, int userid)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "UPDATE");
                dyParam.Add("P_DataPointValue", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_BDODATAID", OracleDbType.Int32, ParameterDirection.Input, dp.BDODATAID);
              //  dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml2);
                //dyParam.Add("P_FromDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.FROMDATE);
                //dyParam.Add("P_ToDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.TODATE);
                //dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(dp.TODATE.Split('-')[2]));
                //dyParam.Add("P_FrequencyValue", OracleDbType.Varchar2, ParameterDirection.Input, dp.FREQUENCYVALUE);
                //dyParam.Add("P_LevelDetailId", OracleDbType.Int32, ParameterDirection.Input, BLOCKID);
                //dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                //dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, dp.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAUPDATE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();

                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
            //return strOutput;
        }
        public string AddBDOData(XElement xml,XElement xml2, Domain.DataPoint.DataPoint dp, int BLOCKID, int FREQUENCYNO, int userid)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, userid);
                dyParam.Add("P_SECTORId", OracleDbType.Int32, ParameterDirection.Input, dp.SECTORID);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_XML_DATA2", OracleDbType.XmlType, ParameterDirection.Input, xml2);
                dyParam.Add("P_FromDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.FROMDATE);
                dyParam.Add("P_ToDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.TODATE);
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(dp.TODATE.Split('-')[2]));
                dyParam.Add("P_FrequencyValue", OracleDbType.Varchar2, ParameterDirection.Input, dp.FREQUENCYVALUE);
                dyParam.Add("P_LevelDetailId", OracleDbType.Int32, ParameterDirection.Input, BLOCKID);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, dp.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAADDUPDATE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();

                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
            //return strOutput;
        }
        //public void BDOTempDataSubmission(Domain.DataPoint.DataPoint dp)
        //{
        //    string strOutput = "";
        //    IEnumerable<SuccessMessage> SuccessMessages;
        //    try
        //    {
        //        var dyParam = new OracleDynamicParameters();
        //        dyParam.Add("P_DataPointId", OracleDbType.Int32, ParameterDirection.Input, dp.DATAPOINTID);
        //        dyParam.Add("P_LevelDetailId", OracleDbType.Int32, ParameterDirection.Input, dp.BLOCKID);
        //        dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, dp.SECTORID);
        //        dyParam.Add("P_Value", OracleDbType.Decimal, ParameterDirection.Input, dp.DATAPOINTVALUE);
        //        dyParam.Add("P_Remark", OracleDbType.Varchar2, ParameterDirection.Input, dp.COLLECTORREMARK);
        //        dyParam.Add("P_FromDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.FROMDATE);
        //        dyParam.Add("P_ToDate", OracleDbType.Varchar2, ParameterDirection.Input, dp.TODATE);
        //        dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(dp.TODATE.Split('-')[2]));
        //        dyParam.Add("P_FrequencyValue", OracleDbType.Varchar2, ParameterDirection.Input, dp.FREQUENCYVALUE);
        //        dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, dp.FREQUENCYNO);
        //        dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, dp.STATUS);
        //        dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, dp.CREATEDBY);
        //        dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
        //        var query = "USP_ABP_BDOTEMPDATAADDUPDATE";
        //        SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
        //        strOutput = SuccessMessages.AsList()[0].successid.ToString();
        //        //return strOutput;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, ex.Message, dp);
        //        throw ex;
        //    }
        //    //return strOutput;
        //}

        public string BDOTempDataSubmission(XElement xml, int STATUS, int userid)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, STATUS);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, userid);
                //dyParam.Add("P_Datapointid", OracleDbType.Int32, ParameterDirection.Input, DATAPOINTID);
                //dyParam.Add("P_FromDate", OracleDbType.Int32, ParameterDirection.Input, FROMDATE);
                //dyParam.Add("P_ToDate", OracleDbType.Int32, ParameterDirection.Input, TODATE);
                //dyParam.Add("P_FrequencyValue", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYVALUE);
                //dyParam.Add("P_Blockid", OracleDbType.Int32, ParameterDirection.Input, BLOCKID);

                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDOTEMPDATAADDUPDATE";
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
        public string BDODataSubmission(XElement xml, int FREQUENCYNO, int FREQUENCYID, int userid, string prevstartdate, string prevenddate)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, 2);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'F');
                dyParam.Add("P_Prevfromdate", OracleDbType.Varchar2, ParameterDirection.Input, prevstartdate);
                dyParam.Add("P_Prevtodate", OracleDbType.Varchar2, ParameterDirection.Input, prevenddate);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, userid);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATASUBMISSION";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
            //return strOutput;
        }
      
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllCollectorApproveData(int LevelId, int UserId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "c");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTORVIEW";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllBDOData(int LevelId,int UserId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'A');
                dyParam.Add("P_LevelId", OracleDbType.Varchar2, ParameterDirection.Input, LevelId);
                dyParam.Add("P_userId", OracleDbType.Varchar2, ParameterDirection.Input, UserId);
                dyParam.Add("P_SECTORID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, 0);
              
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAPOINTVIEW";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllDepartmentDashboard(int LevelId, int sectorid, int frequency, int year)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'B');
                dyParam.Add("P_LevelId", OracleDbType.Varchar2, ParameterDirection.Input, LevelId);
                dyParam.Add("P_SECTORID", OracleDbType.Varchar2, ParameterDirection.Input, sectorid);
                dyParam.Add("P_FREQUENCY", OracleDbType.Varchar2, ParameterDirection.Input, frequency);
                dyParam.Add("P_YEAR", OracleDbType.Varchar2, ParameterDirection.Input, year);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAPOINTVIEW";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> BDODataIdGetById(int SECTORID, string frequencyvalue, int Year)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
               dyParam.Add("P_sectorid", OracleDbType.Int32, ParameterDirection.Input, SECTORID);
               
               dyParam.Add("P_frequencyvalue", OracleDbType.Varchar2, ParameterDirection.Input, frequencyvalue);
               dyParam.Add("P_year", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                 var query = "USP_ABP_GETBDODATAPOINTBYID";
               // var query = "USP_ABP_DPBYSECTORANDFREQ";
                
                  var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> SearchBDOData(int LevelId, int SectorId, int Year, int FrequencyId, string FrequencyValue, string FromDate, string ToDate)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_LevelId", OracleDbType.Varchar2, ParameterDirection.Input, LevelId);
                dyParam.Add("P_SectorId", OracleDbType.Varchar2, ParameterDirection.Input, SectorId);
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, FrequencyId);
                dyParam.Add("P_FrequencyValue", OracleDbType.Varchar2, ParameterDirection.Input, FrequencyValue);
                dyParam.Add("P_FromDate", OracleDbType.Varchar2, ParameterDirection.Input, FromDate);
                dyParam.Add("P_ToDate", OracleDbType.Varchar2, ParameterDirection.Input, ToDate);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAPOINTSEARCH";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetBDODataById(int BDODataId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_BDODataId", OracleDbType.Varchar2, ParameterDirection.Input, BDODataId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETBDODATABYID";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAlldatapoints(int sectorid, int frequency, int Status)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'C');
                dyParam.Add("P_SECTORID", OracleDbType.Varchar2, ParameterDirection.Input, sectorid);
                dyParam.Add("P_FREQUENCY", OracleDbType.Varchar2, ParameterDirection.Input, frequency);
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, Status);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAPOINTVIEW";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllSearchcollectorApprovedata(int sectorid, int frequency, int YEAR, int LeveDetailId, int UserId)
        {


            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "E");
                dyParam.Add("P_SECTORID", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, frequency);
                dyParam.Add("P_LevelId", OracleDbType.Int32, ParameterDirection.Input, LeveDetailId);
                dyParam.Add("P_userId", OracleDbType.Int32, ParameterDirection.Input, UserId);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAPOINTVIEW";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        public async Task<IEnumerable<Domain.DataPoint.DataPoint>> GetAllSearchbdodata(int sectorid, int frequency, int YEAR, int LeveDetailId,int UserId)
        {


            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "A");
                dyParam.Add("P_SECTORID", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, frequency);
                dyParam.Add("P_LevelId", OracleDbType.Int32, ParameterDirection.Input, LeveDetailId);
                dyParam.Add("P_userId", OracleDbType.Int32, ParameterDirection.Input, UserId);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAPOINTVIEW";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        public async Task<IEnumerable<ABP.Domain.Indicator.Indicator>> Getdatapoints(int sectorid, int frequency, int Status)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'D');
                dyParam.Add("P_SECTORID", OracleDbType.Varchar2, ParameterDirection.Input, sectorid);
                dyParam.Add("P_FREQUENCY", OracleDbType.Varchar2, ParameterDirection.Input, frequency);
                dyParam.Add("P_Status", OracleDbType.Int32, ParameterDirection.Input, Status);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATAPOINTVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<IndicatorMapping>> GetDataPointID(int INDICATORID)
        {
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
            dyParam.Add("P_indicatorid", OracleDbType.Int32, ParameterDirection.Input, INDICATORID);
            var query = "USP_GET_DATAPOINT";
            var result = await Connection.QueryAsync<IndicatorMapping>(query, dyParam, commandType: CommandType.StoredProcedure);
            return result;
        }
        public string TrackOTP(ABP.Domain.Login.OTPTracker.OTPTracker OTP)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_MobileNo", OracleDbType.Varchar2, ParameterDirection.Input, OTP.MOBILENO);
                dyParam.Add("P_OTP", OracleDbType.Int32, ParameterDirection.Input, OTP.OTP);
                dyParam.Add("P_OTPType", OracleDbType.Int32, ParameterDirection.Input, OTP.OTPTYPE);
                dyParam.Add("P_IPAddress", OracleDbType.Varchar2, ParameterDirection.Input, OTP.IPADDRESS);
                dyParam.Add("P_Userid", OracleDbType.Int32, ParameterDirection.Input, OTP.CREATEDBY);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_OTP_TRACKER";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
            //return strOutput;
        }

        public async Task<int> BDODATACOUNT(int FREQUENCYNO)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'A');
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                var query = "USP_ABP_BDODATASUBMISSIONCOUNT";
                var result = await Connection.QueryFirstOrDefaultAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> MONTHDATACOUNT(int FreqId,int FREQUENCYNO)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'M');
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                var query = "USP_ABP_BDODATASUBMISSIONCOUNT";
                var result = await Connection.QueryFirstOrDefaultAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NEWBDODataSubmission(int FREQUENCYNO, string prevstartdate, string prevenddate, XElement xml)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'N');
                dyParam.Add("P_Prevfromdate", OracleDbType.Varchar2, ParameterDirection.Input, prevstartdate);
                dyParam.Add("P_Prevtodate", OracleDbType.Varchar2, ParameterDirection.Input, prevenddate);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, xml);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDODATASUBMISSION";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
            //return strOutput;
        }
        public string GetLastFreq(int LeveDetailId,int SectorId, int FreqId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, LeveDetailId);
                dyParam.Add("P_SECTORID", OracleDbType.Int32, ParameterDirection.Input, SectorId);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETLASTFREQNO";
                var result =  Connection.QueryFirstOrDefault<string>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<int> FRESHDATACOUNT(int FreqId, int FREQUENCYNO)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, 'C');
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                var query = "USP_ABP_BDODATASUBMISSIONCOUNT";
                var result = await Connection.QueryFirstOrDefaultAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;

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
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "C");
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
        public Task<IEnumerable<ABP.Domain.CollectorData.CollectorDataAlert>> GetCollecterAlertData(int districid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, districid);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTOR_ALERT";
                var result = Connection.QueryAsync<ABP.Domain.CollectorData.CollectorDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task<IEnumerable<ABP.Domain.CollectorData.CollectorDataAlert>> GetDeptAlertData(int districid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DEPT");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, districid);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DEPT_ALERT";
                var result = Connection.QueryAsync<ABP.Domain.CollectorData.CollectorDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task<IEnumerable<ABP.Domain.CollectorData.CollectorDataAlert>> GetMonYearData(int blockid,string Applicationbo)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DEPT");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Int32, ParameterDirection.Input, Applicationbo);
                
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DPMONYEAR";
                var result = Connection.QueryAsync<ABP.Domain.CollectorData.CollectorDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
