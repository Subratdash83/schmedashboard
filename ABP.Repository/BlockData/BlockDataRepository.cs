using ABP.Domain;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.BlockData
{
    public class BlockDataRepository : RepositoryBase, IBlockDataRepository
    {
        public IConfiguration Configuration { get; }
        public BlockDataRepository(IConnectionFactory connectionFactory, IConfiguration configuration) : base(connectionFactory)
        {
            Configuration = configuration;
        }
        public async Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByFrequency(ABP.Domain.Panel.ControlPanel cp)
        {
            try
            {

                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GCF");
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, cp.YEARTYPE);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetYearlyAppNo(ABP.Domain.Panel.ControlPanel cp)
        //{
        //    try
        //    {

        //        var dyParam = new OracleDynamicParameters();
        //        dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
        //        dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "YAN");
        //        //dyParam.Add("P_DPMONTH", OracleDbType.Int32, ParameterDirection.Input, cp.DPMONTH);
        //        //dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, cp.BLOCKID);
        //        //dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, cp.YEAR);
        //        var query = "USP_ABP_BLOCKDATA_MANAGE";
        //        var result = await Connection.QueryAsync<ABP.Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        
        public async Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByFrequencyMY(ABP.Domain.Panel.ControlPanel cp)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GCFMY");
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, cp.YEARTYPE);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetLastFreq(int LeveDetailId, int SectorId, int FreqId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GFQ");
                //dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, SectorId);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, LeveDetailId);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = Connection.QueryFirstOrDefault<string>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string GetYearlyAppNo(int BLOCKID, int DPMONTH, int YEAR)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "YAN");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, BLOCKID);
                dyParam.Add("P_DPMONTH", OracleDbType.Int32, ParameterDirection.Input, DPMONTH);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, YEAR);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = Connection.QueryFirstOrDefault<string>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> FRESHDATACOUNT(int FreqId, int FREQUENCYNO, int BlockId,int Year)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "C");
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, BlockId);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Year);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryFirstOrDefaultAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> FREQDATACOUNT(int FreqId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DABF");
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryFirstOrDefaultAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> MONTHDATACOUNT(int FreqId, int FREQUENCYNO, int BlockId, int Year)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "M");
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, BlockId);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryFirstOrDefaultAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SubmitBlockData(List<StringBuilder> sb)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    foreach (var item in sb)
                    {
                        Log.Information("Query executing--"+ item.ToString());
                        using (OracleCommand cmd = new OracleCommand(item.ToString(), conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return "1";
            }
            catch(Exception ex)
            {
                Log.Information(ex.Message);
                throw ex;
                //return "2";
            }
        }
        public string AddBlockDataMain(ABP.Domain.BlockData.BlockData bd)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INSERT");
                dyParam.Add("P_DPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.DPCOUNT);
                dyParam.Add("P_NONZERODPCOUNT", OracleDbType.Varchar2, ParameterDirection.Input, bd.NONZERODPCOUNT);
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_FREQUENCYVALUE", OracleDbType.Varchar2, ParameterDirection.Input, bd.FREQUENCYVALUE);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_FROMDATE", OracleDbType.Varchar2, ParameterDirection.Input, bd.FROMDATE);
                dyParam.Add("P_TODATE", OracleDbType.Varchar2, ParameterDirection.Input, bd.TODATE);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, bd.DISTRICTID);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_CREATEDBY", OracleDbType.Int32, ParameterDirection.Input, bd.CREATEDBY);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_HNDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.HNDPCOUNT);
                dyParam.Add("P_AGDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.AGDPCOUNT);
                dyParam.Add("P_BIDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.BIDPCOUNT);
                dyParam.Add("P_EDDPCOUNT ", OracleDbType.Int32, ParameterDirection.Input, bd.EDDPCOUNT);
                dyParam.Add("P_FIDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.FIDPCOUNT);
                dyParam.Add("P_MONTHNO", OracleDbType.Int32, ParameterDirection.Input, bd.MONTHNO);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string UpdateBlockDataMain(ABP.Domain.BlockData.BlockData bd)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "UPDATE");
                dyParam.Add("P_DPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.DPCOUNT);
                dyParam.Add("P_NONZERODPCOUNT", OracleDbType.Varchar2, ParameterDirection.Input, bd.NONZERODPCOUNT);
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_CREATEDBY", OracleDbType.Int32, ParameterDirection.Input, bd.CREATEDBY);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_HNDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.HNDPCOUNT);
                dyParam.Add("P_AGDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.AGDPCOUNT);
                dyParam.Add("P_BIDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.BIDPCOUNT);
                dyParam.Add("P_EDDPCOUNT ", OracleDbType.Int32, ParameterDirection.Input, bd.EDDPCOUNT);
                dyParam.Add("P_FIDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.FIDPCOUNT);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateYearlyApplicationNumber(ABP.Domain.BlockData.BlockData bd)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "UYAPPNO");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockData(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(bd.APPLICATIONNO))
                {
                    bd.APPLICATIONNO = "0";
                }
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                if(bd.FREQUENCYID == 2)
                {
                    dyParam.Add("P_MONTHNO", OracleDbType.Int32, ParameterDirection.Input, 0);
                }
                else
                {
                    dyParam.Add("P_MONTHNO", OracleDbType.Int32, ParameterDirection.Input, bd.MONTHNO);
                }
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockDataDet(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bd.APPLICATIONNO))
                {
                    bd.APPLICATIONNO = "0";
                }
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEW");
               
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                if (bd.FREQUENCYID == 2)
                {
                    dyParam.Add("P_MONTHNO", OracleDbType.Int32, ParameterDirection.Input, 0);
                }
                else
                {
                    dyParam.Add("P_MONTHNO", OracleDbType.Int32, ParameterDirection.Input, bd.MONTHNO);
                }
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  Task<IEnumerable<ABP.Domain.BlockData.BlockDataAlert>> GetBDOAlertData(int blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BDOSEL");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input,blockid);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDO_ALERT";
                var result =  Connection.QueryAsync<ABP.Domain.BlockData.BlockDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
        }
        public Task<IEnumerable<ABP.Domain.BlockData.BlockDataAlert>> GetBDOAlertDataPoint(int blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BDOSEL");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BDO_ALERT";
                var result = Connection.QueryAsync<ABP.Domain.BlockData.BlockDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockDataByAppNo(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bd.APPLICATIONNO))
                {
                    bd.APPLICATIONNO = "0";
                }
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "BDAPPNO");
                //dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);

                //--------------------------------Changes for Testing-----------------------------------------------

                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "APPNO");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);

                //--------------------------------Changes for Testing-----------------------------------------------
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DATAPOINT_VIEWS";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockDatas(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bd.APPLICATIONNO))
                {
                    bd.APPLICATIONNO = "0";
                }
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "vw");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETAPPNO";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetAllBlockData(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                if (bd.APPLICATIONNO == null)
                {
                    bd.APPLICATIONNO = "0";
                }
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEWALL");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_PANELID", OracleDbType.Int32, ParameterDirection.Input, bd.PANELID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetAllBlockDataForAutoApproval()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEWALLAPP");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
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
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> Getdatapoints(int sectorid, int frequency, int Status)
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
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public decimal? GetContolValue(ABP.Domain.Panel.ControlPanel cp, string AppNo)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("select " + cp.CONTROLNAME + " as " + cp.CONTROLNAME + " from T_ABP_" + cp.PANELNAME + " dt left join T_ABP_BLOCK_DATAENTRY bd on dt.APPLICATIONNO=bd.APPLICATIONNO where dt.ApplicationNo='" + AppNo + "'", conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                return Convert.ToDecimal(dt.Rows[0][cp.CONTROLNAME]);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public decimal? GetContolValuecol(ABP.Domain.Panel.ControlPanel cp, string AppNo, int BLOCKID, int YEAR, int FREQUENCYID, int FREQUENCYNO)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string applno = "";
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    if(FREQUENCYID==2)
                    {
                        using (OracleCommand cmd = new OracleCommand("select " + cp.CONTROLNAME + " as " + cp.CONTROLNAME + " from T_ABP_" + cp.PANELNAME + " dt left join T_ABP_BLOCK_DATAENTRY bd on dt.APPLICATIONNO=bd.APPLICATIONNO where dt.ApplicationNo='" + AppNo + "'", conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }              
                        return Convert.ToDecimal(dt.Rows[0][cp.CONTROLNAME]);
                    }
                    else
                    {

                        string Applicationno = ("select applicationno from t_abp_block_dataentry where frequencyid = " + FREQUENCYID + " and year = " + YEAR + " and blockid = " + BLOCKID + " and dpmonth=" + FREQUENCYNO + "");
                        using (OracleCommand cmd1 = new OracleCommand(Applicationno, conn))
                        {
                            using (OracleDataReader reader = cmd1.ExecuteReader())
                            {
                                dt1.Load(reader);
                            }                           
                        }
                        if(dt1.Rows.Count > 0)
                            {
                            applno = Convert.ToString(dt1.Rows[0][dt1.Columns[0]]);
                        }
                      
                        if (applno =="")
                        {
                            int prevyear = YEAR - 1;
                            string Applicationno1 = ("select * from (select APPLICATIONNO from t_abp_block_dataentry where frequencyid = "+ FREQUENCYID+" and year <= "+ prevyear + " and blockid = "+BLOCKID +" order by id desc)tabp where rownum = 1");
                            using (OracleCommand cmd1 = new OracleCommand(Applicationno1, conn))
                            {
                                using (OracleDataReader reader = cmd1.ExecuteReader())
                                {
                                    dt1.Load(reader);
                                }
                            }
                           string applno1 = Convert.ToString(dt1.Rows[0][dt1.Columns[0]]);
                            applno = Convert.ToString(dt1.Rows[0][dt1.Columns[0]]); 
                        }
                        string cpvalue = "select " + cp.CONTROLNAME + " as " + cp.CONTROLNAME + " from T_ABP_" + cp.PANELNAME + " dt left join T_ABP_BLOCK_DATAENTRY bd on dt.APPLICATIONNO=bd.APPLICATIONNO where dt.ApplicationNo='" + applno + "'";
                        using (OracleCommand cmd = new OracleCommand(cpvalue, conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                  
                }
                return Convert.ToDecimal(dt.Rows[0][cp.CONTROLNAME]);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public decimal? GetContolValueyear(ABP.Domain.Panel.ControlPanel cp, string AppNo, int BLOCKID, int YEAR, int FREQUENCYID, int FREQUENCYNO)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string applno = "";
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    if (FREQUENCYID == 2)
                    {
                        using (OracleCommand cmd = new OracleCommand("select " + cp.CONTROLNAME + " as " + cp.CONTROLNAME + " from T_ABP_" + cp.PANELNAME + " dt left join T_ABP_BLOCK_DATAENTRY bd on dt.APPLICATIONNO=bd.APPLICATIONNO where dt.ApplicationNo='" + AppNo + "'", conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                        return Convert.ToDecimal(dt.Rows[0][cp.CONTROLNAME]);
                    }
                    else
                    {
                        string cpvalue = ("SELECT * FROM( SELECT " + cp.CONTROLNAME + " FROM T_ABP_" + cp.PANELNAME + " WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH || '-' || year, 'mm-yyyy') <= to_date('" + FREQUENCYNO + "-" + YEAR + "', 'mm-yyyy') AND BLOCKID = " + BLOCKID + " and status in (1, 2)) ORDER BY ID DESC ) WHERE ROWNUM = 1 AND " + cp.CONTROLNAME + " IS NOT NULL");
                        using (OracleCommand cmd = new OracleCommand(cpvalue, conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                       
                    }

                }
                return Convert.ToDecimal(dt.Rows[0][cp.CONTROLNAME]);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public decimal? GetContolValuecolyear(ABP.Domain.Panel.ControlPanel cp, string AppNo, int BLOCKID, int YEAR, int FREQUENCYID, int FREQUENCYNO)
        {
            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string applno = "";
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    if (FREQUENCYID == 2)
                    {
                        using (OracleCommand cmd = new OracleCommand("select " + cp.CONTROLNAME + " as " + cp.CONTROLNAME + " from T_ABP_" + cp.PANELNAME + " dt left join T_ABP_BLOCK_DATAENTRY bd on dt.APPLICATIONNO=bd.APPLICATIONNO where dt.ApplicationNo='" + AppNo + "'", conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }
                        return Convert.ToDecimal(dt.Rows[0][cp.CONTROLNAME]);
                    }
                    else
                    {
                        string cpvalue = ("SELECT * FROM( SELECT " + cp.CONTROLNAME + " FROM T_ABP_" + cp.PANELNAME + " WHERE Applicationno IN(SELECT APPLICATIONNO FROM T_ABP_BLOCK_DATAENTRY WHERE FREQUENCYID = 5 AND to_date(DPMONTH || '-' || year, 'mm-yyyy') <= to_date('" + FREQUENCYNO + "-" + YEAR + "', 'mm-yyyy') AND BLOCKID = " + BLOCKID + " and status=1) ORDER BY ID DESC ) WHERE ROWNUM = 1 AND " + cp.CONTROLNAME + " IS NOT NULL");
                        using (OracleCommand cmd = new OracleCommand(cpvalue, conn))
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                dt.Load(reader);
                            }
                        }

                    }

                }
                return Convert.ToDecimal(dt.Rows[0][cp.CONTROLNAME]);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public decimal GetCountValue(string PANELNAME, string AppNo)
        {
            try
            {
                var logtablename = PANELNAME + "_LOG";
                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("select nvl(count(1),0) as logcount from T_ABP_" + logtablename + "  where ApplicationNo='" + AppNo + "'", conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                return Convert.ToDecimal(dt.Rows[0]["logcount"]);
            }
            catch (Exception ex)
            {
                return Convert.ToDecimal(0);
            }
        }

        public async Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetRejectedControls(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GCFR");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                if (bd.FREQUENCYID == 5)
                {
                    dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.DPMONTH);
                }else
                {
                    dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                } 
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetFromDateToDate(int FreqId, int MONTH, int BlockId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DERUL");
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, MONTH);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, BlockId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Frequency.FrequencyDP>> GetFrequencyValue(ABP.Domain.BlockData.BlockData data)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DERUL");
                 
                //dyParam.Add("P_APPLICATIONNO", OracleDbType.Int32, ParameterDirection.Input, data.APPLICATIONNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, data.BLOCKID);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, data.FREQUENCYID);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, data.YEAR);
                dyParam.Add("P_FREQNO", OracleDbType.Int32, ParameterDirection.Input, data.FREQUENCYNO);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETFREQUENCYDETAILS";
                var result = await Connection.QueryAsync<ABP.Domain.Frequency.FrequencyDP>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetMonthlyFreezeDashboardData(ABP.Domain.BlockData.BlockData data)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DERUL");

                //dyParam.Add("P_APPLICATIONNO", OracleDbType.Int32, ParameterDirection.Input, data.APPLICATIONNO);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "VIEWSTATUS");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, data.BLOCKID);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, data.DISTRICTID);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, data.FREQUENCYID);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, data.YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateFreezeDashboardData(string ApplicationNo, int Blockid, int Districtid, int Status,int year,int frequencyid,int frequencyno, int Userid)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "UPDATESTS");
              
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, ApplicationNo);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Districtid);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, Status);
                dyParam.Add("P_CREATEDBY", OracleDbType.Int32, ParameterDirection.Input, Userid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, frequencyid);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, frequencyno);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> ViewFreezedetails(int DISTRICTID, int BLOCKID)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VWFREZE");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, DISTRICTID);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input,BLOCKID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Frequency.FrequencyDP>> GetFrequencyValuemoth(int frquencyid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DERUL");

                dyParam.Add("P_APPLICATIONNO", OracleDbType.Int32, ParameterDirection.Input, null);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, frquencyid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_FREQNO", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETFREQUENCYDETAILS";
                var result = await Connection.QueryAsync<ABP.Domain.Frequency.FrequencyDP>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetEnteredDatapoints(string AppNo,string PanelName)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, AppNo);
                dyParam.Add("P_tbl_name", OracleDbType.Varchar2, ParameterDirection.Input, PanelName);
                dyParam.Add("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_UNPIVOT_DYNAMIC";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetAllControlPanelDP(string PanelName, int FreqId, int FreqNo)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_PANELNAME", OracleDbType.Varchar2, ParameterDirection.Input, PanelName);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, FreqId);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, FreqNo);
                dyParam.Add("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_CONTROLPANELDP";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> DATA()
        {
            try
            {
                
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DATA");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByFrequencyearly(ABP.Domain.Panel.ControlPanel cp)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BLKY");
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, cp.YEARTYPE);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string AddBlockDataMainTmp(ABP.Domain.BlockData.BlockData bd)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INSERT1");
                dyParam.Add("P_DPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.DPCOUNT);
                dyParam.Add("P_NONZERODPCOUNT", OracleDbType.Varchar2, ParameterDirection.Input, bd.NONZERODPCOUNT);
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_FREQUENCYID", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_FREQUENCYVALUE", OracleDbType.Varchar2, ParameterDirection.Input, bd.FREQUENCYVALUE);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_FROMDATE", OracleDbType.Varchar2, ParameterDirection.Input, bd.FROMDATE);
                dyParam.Add("P_TODATE", OracleDbType.Varchar2, ParameterDirection.Input, bd.TODATE);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, bd.DISTRICTID);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_CREATEDBY", OracleDbType.Int32, ParameterDirection.Input, bd.CREATEDBY);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_HNDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.HNDPCOUNT);
                dyParam.Add("P_AGDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.AGDPCOUNT);
                dyParam.Add("P_BIDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.BIDPCOUNT);
                dyParam.Add("P_EDDPCOUNT ", OracleDbType.Int32, ParameterDirection.Input, bd.EDDPCOUNT);
                dyParam.Add("P_FIDPCOUNT", OracleDbType.Int32, ParameterDirection.Input, bd.FIDPCOUNT);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockDataTmp(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bd.APPLICATIONNO))
                {
                    bd.APPLICATIONNO = "0";
                }
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "VIEWCMN");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FrequencyNo", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string BlockDatapointYearlyCheck(int Year,int Blockid,int Frequencyid)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DPCHK");
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, Year);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, Frequencyid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.BlockData.BlockData>> GetBlockEnteredDataByYear(ABP.Domain.BlockData.BlockData bd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bd.APPLICATIONNO))
                {
                    bd.APPLICATIONNO = "0";
                }
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "YEARVW");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, bd.APPLICATIONNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, bd.BLOCKID);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, bd.STATUS);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, bd.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, bd.YEAR);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETAPPNO";
                var result = await Connection.QueryAsync<ABP.Domain.BlockData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Panel.ControlPanel>> GetControlsByIndicators(ABP.Domain.Panel.ControlPanel cp)
        {
            try
            {

                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INDP");
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYID);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, cp.FREQUENCYNO);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, cp.YEARTYPE);
                dyParam.Add("P_INDICATORID", OracleDbType.Int32, ParameterDirection.Input, cp.INDICATORID);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.Panel.ControlPanel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateRejData(string Applino)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "REJUPD");
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Varchar2, ParameterDirection.Input, Applino);
                
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
        public string GetRejectAppno(int Year,int FREQUENCYNO,int Blokid,int FrequencyId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();               
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "APPNOSEL");
                dyParam.Add("P_YEAR", OracleDbType.Varchar2, ParameterDirection.Input, Year);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Varchar2, ParameterDirection.Input, FREQUENCYNO);
                dyParam.Add("P_BLOCKID", OracleDbType.Varchar2, ParameterDirection.Input, Blokid);
                dyParam.Add("P_FrequencyId", OracleDbType.Varchar2, ParameterDirection.Input, FrequencyId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = Connection.QueryFirstOrDefault<string>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> ViewRejectedBlockDetails(int BLOCKID)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "BLKREJ");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, BLOCKID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDATA_MANAGE";
                var result = await Connection.QueryAsync<ABP.Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

