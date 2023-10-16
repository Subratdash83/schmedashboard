using ABP.Domain;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ABP.Domain.Indicator;
using ABP.Domain.Sector;
using Microsoft.Extensions.Configuration;

namespace ABP.Repository.Indicator
{
    public class IndicatorRepository : RepositoryBase, IIndicatorRepository
    {
        private readonly ILogger<IndicatorRepository> Logger;
        public IConfiguration Configuration { get; }
        public IndicatorRepository(IConnectionFactory connectionFactory, IConfiguration configuration, ILogger<IndicatorRepository> logger) : base(connectionFactory)
        {
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("IndicatorRepository initialized");
        }

        public string InsertIndicator(Domain.Indicator.Indicator indicator)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (indicator.INDICATORID != 0)
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                }
                else
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "INSERT"); ;
                }
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, indicator.SECTORID);
                dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, indicator.INDICATORID);
                dyParam.Add("P_IndicatorName", OracleDbType.Varchar2, ParameterDirection.Input, indicator.INDICATORNAME);
                dyParam.Add("P_IndicatorType", OracleDbType.Int32, ParameterDirection.Input, indicator.INDICATORTYPE);
                dyParam.Add("P_Description", OracleDbType.Varchar2, ParameterDirection.Input, indicator.DESCRIPTION);
                dyParam.Add("P_IndicatorDate", OracleDbType.Varchar2, ParameterDirection.Input, indicator.INDICATORDATE);
                dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, indicator.CREATEDBY);
                dyParam.Add("P_FREQUENCY", OracleDbType.Int32, ParameterDirection.Input, indicator.FREQUENCYID);
                dyParam.Add("P_TargetValue", OracleDbType.Int32, ParameterDirection.Input, indicator.TARGETVALUE);
                dyParam.Add("P_IndType", OracleDbType.Int32, ParameterDirection.Input, indicator.IndType);
                dyParam.Add("P_Unit", OracleDbType.Varchar2, ParameterDirection.Input, indicator.UNIT);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORADDUPDATE";
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

        public async Task<IEnumerable<Domain.Indicator.Indicator>> ViewIndicator(int sectorid, int indicatorid, int frequencyid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_Sectorid", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, frequencyid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORVIEWDELETE";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DeleteIndicator(Domain.Indicator.Indicator Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, Model.INDICATORID);
                p.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORVIEWDELETE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorGateById(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYID";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public string AddIndicatorMapping(Domain.Indicator.IndicatorMapping indicatormapping)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (indicatormapping.iscensus != 0)
                {
                    dyParam.Add("P_DDataPointId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.DDATAPOINTID);
                    dyParam.Add("P_NDataPointId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.DDATAPOINTID);
                }
                else
                {
                    dyParam.Add("P_DDataPointId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.DDATAPOINTID);
                    dyParam.Add("P_NDataPointId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.NDATAPOINTID);
                }
                if (indicatormapping.INDICATORMAPPINGID != 0)
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                    dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.SECTORID);
                    dyParam.Add("P_IndicatorMappingId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.INDICATORMAPPINGID);
                    dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.INDICATORID);
                    dyParam.Add("P_Formula", OracleDbType.Varchar2, ParameterDirection.Input, indicatormapping.FORMULA);
                    dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.CREATEDBY);
                    dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                    var query = "USP_ABP_IMADDUPDATE";
                    SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                    strOutput = SuccessMessages.AsList()[0].successid.ToString();
                }
                else
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "INSERT");
                    dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.SECTORID);
                    dyParam.Add("P_IndicatorMappingId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.INDICATORMAPPINGID);
                    dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.INDICATORID);
                    dyParam.Add("P_Formula", OracleDbType.Varchar2, ParameterDirection.Input, indicatormapping.FORMULA);
                    dyParam.Add("P_UserId", OracleDbType.Int32, ParameterDirection.Input, indicatormapping.CREATEDBY);
                    dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                    var query = "USP_ABP_IMADDUPDATE";
                    SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                    strOutput = SuccessMessages.AsList()[0].successid.ToString();
                }
                return strOutput;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, indicatormapping);
                throw ex;
            }
            //return strOutput;
        }

        public async Task<IEnumerable<Domain.Indicator.IndicatorMapping>> ViewIndicatorMapping(string ACTION, int sectorid, int indicatorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, ACTION);
                dyParam.Add("P_Sectorid", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_Indicatorid", OracleDbType.Int32, ParameterDirection.Input, indicatorid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_IMVIEWDELETE";
                var result = await Connection.QueryAsync<Domain.Indicator.IndicatorMapping>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorGateBySectorId(int id,int fid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, fid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETINDICATORBYSECTORID";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public string DeleteIndicatorMapping(Domain.Indicator.IndicatorMapping Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_IndicatorMappingId", OracleDbType.Int32, ParameterDirection.Input, Model.INDICATORMAPPINGID);
                p.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_IMVIEWDELETE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.IndicatorMapping>> IndicatorMappingGateByIMId(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_IndicatorMappingId", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETIMBYID";
                var result = await Connection.QueryAsync<Domain.Indicator.IndicatorMapping>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorsGPBySector(int Frequency, int BlockId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Frequency", OracleDbType.Int32, ParameterDirection.Input, Frequency);
                dyParam.Add("P_LevelDetailId", OracleDbType.Int32, ParameterDirection.Input, BlockId);
                //dyParam.Add("P_Frequency", OracleDbType.Int32, ParameterDirection.Input, Frequency);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORSBYSECTOR";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorsByBlockIdGPBySector(int BlockId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_LevelDetailId", OracleDbType.Int32, ParameterDirection.Input, BlockId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORSBYBLOCKID";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Indicator.Indicator>> GETALLIndicators(int Frequency)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETPANELINDICATOR";
                var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public List<Domain.Indicator.Indicator> GETALLDP(string applicationNo, int Frequency)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ApplicationNo", OracleDbType.Varchar2, ParameterDirection.Input, applicationNo);
                var query = "USP_ABP_GETDPVALUES";
                var result = Connection.Query<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure).AsList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Indicator.IndicatorMapping>> IndicatorMappingGateBySectorAndIMId(int SectorId, int IMId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, SectorId);
                dyParam.Add("P_IndicatorId", OracleDbType.Int32, ParameterDirection.Input, IMId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETIMBYSECTORANDIMID";
                var result = await Connection.QueryAsync<Domain.Indicator.IndicatorMapping>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }


        public string AddUpdateIndicatorsFormula(Domain.Indicator.Indicator indicator)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();


                dyParam.Add("P_SECTORId", OracleDbType.Int32, ParameterDirection.Input, indicator.SECTORID);
                dyParam.Add("P_USERID", OracleDbType.Int32, ParameterDirection.Input, indicator.CREATEDBY);
                dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, indicator.ApprovalConfing);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORFORMULA_ADD";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();


            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }
        //public async Task<IEnumerable<Domain.Indicator.Indicator>> IndicatorsGPBySector()
        //{
        //    try
        //    {
        //        var dyParam = new OracleDynamicParameters();
        //        dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
        //        var query = "USP_ABP_INDICATORSGPBYSECTOR";
        //        var result = await Connection.QueryAsync<Domain.Indicator.Indicator>(query, dyParam, commandType: CommandType.StoredProcedure);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, ex.Message);
        //        throw ex;
        //    }
        //}
        public decimal GetContolValueThroughQuery(string Query)
        {
            try
            {

                try
                {
                    DataTable dt = new DataTable();
                    using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(Query, conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    return Convert.ToDecimal(dt.Rows[0][dt.Columns[0]]);
                }
                catch(Exception ex)
                {
                    if(ex.Message.Contains(":"))
                    { 
                    string[] message2 = ex.Message.Split(':');
                    string message3 = message2[1].ToString();
                    //string message2[] = ex.Message.Split(':');
                       if (message3== " divisor is equal to zero")
                        {
                         return -99;
                        }
                        else
                        {
                          return -99;
                        }
                        //*******Return changes 0 to 99******Auther Subrat****//
                    }
                    else
                    {
                        return -99;
                    }
                }
               
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public decimal GetContolValueThroughQueryindscore(string Query, int blockid, int year, int frequencyid, int FREQUENCYNO)
        {
            try
            {

                try
                {
                    DataTable dt = new DataTable();
                    using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(Query, conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    return Convert.ToDecimal(dt.Rows[0][dt.Columns[0]]);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains(":"))
                    {
                        string[] message2 = ex.Message.Split(':');
                        string message3 = message2[1].ToString();
                        //string message2[] = ex.Message.Split(':');
                        if (message3 == " divisor is equal to zero")
                        {
                            return -99;
                        }
                        else
                        {
                            try
                            {
                                DataTable dt1 = new DataTable();
                                int prevyear = year - 1;
                                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                                {
                                    //frequencyid=5 and year=2023 and dpmonth=7
                                    string Query2 = Query.Replace("frequencyid=5 and year=" + year + " and dpmonth=" + FREQUENCYNO + " ", "frequencyid=5 and year=" + prevyear + " ");
                                    conn.Open();
                                    using (OracleCommand cmd = new OracleCommand(Query2, conn))
                                    {
                                        using (OracleDataReader reader = cmd.ExecuteReader())
                                        {
                                            dt1.Load(reader);
                                        }
                                    }
                                }
                                return Convert.ToDecimal(dt1.Rows[0][dt1.Columns[0]]);
                            }
                            catch (Exception ex2)
                            {
                                return -99;
                            }



                        }
                        //*******Return changes 0 to 99******Auther Subrat****//
                    }
                    else
                    {
                        try
                        {


                            DataTable dt1 = new DataTable();
                            DataTable dt2 = new DataTable();
                            DataTable dt3 = new DataTable();
                            int prevyear = year - 1;

                            using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                            {

                                //string Query2 = Query.Replace("frequencyid=5 and year=" + year + " and and dpmonth=" + FREQUENCYNO + " ", "frequencyid=5 and year=" + prevyear + " ");
                                string query3 = "SELECT max(id) as id FROM t_abp_block_dataentry WHERE frequencyid = 5 AND year <= " + prevyear + " AND blockid = " + blockid + " ";
                                conn.Open();
                                using (OracleCommand cmd = new OracleCommand(query3, conn))
                                {
                                    using (OracleDataReader reader = cmd.ExecuteReader())
                                    {
                                        dt1.Load(reader);
                                    }
                                }
                            }
                            string idno = Convert.ToString(dt1.Rows[0][dt1.Columns[0]]);
                            if (idno != "")
                            {
                                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                                {
                                    
                                    //frequencyid=5 and year=2023 and dpmonth=7
                                    string Query2 = Query.Replace("frequencyid=5 and year=" + year + " and dpmonth=" + FREQUENCYNO + " ", "frequencyid=5 and year <=" + prevyear + " and id=" + Convert.ToInt32(idno) + " ");
                                    conn.Open();
                                    using (OracleCommand cmd = new OracleCommand(Query2, conn))
                                    {
                                        using (OracleDataReader reader = cmd.ExecuteReader())
                                        {
                                            dt3.Load(reader);
                                        }
                                    }
                                }

                            }
                            return Convert.ToDecimal(dt3.Rows[0][dt3.Columns[0]]);

                            //return -99;
                        }
                        catch (Exception ex3)
                        {
                            return -99;
                        }
                    }



                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public decimal GetContolValueThroughQueryindYearly(string Query, int blockid, int year, int frequencyid, int FREQUENCYNO)
        {
            try
            {

                try
                {
                    DataTable dt = new DataTable();
                    using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand(Query, conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    return Convert.ToDecimal(dt.Rows[0][dt.Columns[0]]);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains(":"))
                    {
                        string[] message2 = ex.Message.Split(':');
                        string message3 = message2[1].ToString();
                        //string message2[] = ex.Message.Split(':');
                        if (message3 == " divisor is equal to zero")
                        {
                            return -99;
                        }
                        else
                        {
                            try
                            {
                                DataTable dt1 = new DataTable();
                                int prevyear = year - 1;
                                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                                {
                                    //frequencyid=5 and year=2023 and dpmonth=7
                                    string Query2 = Query.Replace("frequencyid=5 and year=" + year + " and dpmonth=" + FREQUENCYNO + " ", "frequencyid=5 and year=" + prevyear + " ");
                                    conn.Open();
                                    using (OracleCommand cmd = new OracleCommand(Query2, conn))
                                    {
                                        using (OracleDataReader reader = cmd.ExecuteReader())
                                        {
                                            dt1.Load(reader);
                                        }
                                    }
                                }
                                return Convert.ToDecimal(dt1.Rows[0][dt1.Columns[0]]);
                            }
                            catch (Exception ex2)
                            {
                                return -99;
                            }
                        }
                        //*******Return changes 0 to 99******Auther Subrat****//
                    }
                    else
                    {
                        try
                        {
                            DataTable dt1 = new DataTable();
                            int prevyear = year - 1;
                            using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                            {
                                //frequencyid=5 and year=2023 and dpmonth=7
                                string Query2 = Query.Replace("frequencyid=5 and year=" + year + " and dpmonth=" + FREQUENCYNO + " ", "frequencyid=5 and year=" + prevyear + " ");
                                conn.Open();
                                using (OracleCommand cmd = new OracleCommand(Query2, conn))
                                {
                                    using (OracleDataReader reader = cmd.ExecuteReader())
                                    {
                                        dt1.Load(reader);
                                    }
                                }
                            }
                            return Convert.ToDecimal(dt1.Rows[0][dt1.Columns[0]]);
                        }
                        catch (Exception ex2)
                        {
                            return -99;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetAppValueThroughQuery(string Query)
        {  
            try
               {
                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(Query, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToString(dt.Rows[0]["APPLICATIONNO"]);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetContolThroughQuery(string Query)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(Query, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToString(dt.Rows[0]["CONTROLVLUE"]);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<IEnumerable<IndicatorValue>> IndicatorValues()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORVAL";
                var result = Connection.QueryAsync<Domain.Indicator.IndicatorValue>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
