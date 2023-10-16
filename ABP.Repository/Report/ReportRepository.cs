using ABP.Domain.BlockMap;
using ABP.Domain.Report;
using ABP.Repository.Contract.Factory;
using ABP.Repository.Contract.Report;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Report
{
  public  class ReportRepository : RepositoryBase, IReportRepository
    {
        public IConfiguration Configuration { get; }
        public ReportRepository(IConnectionFactory connectionFactory, IConfiguration configuration) : base(connectionFactory)
        {
            Configuration = configuration;
        }

        public async Task<IEnumerable<ABP.Domain.Report.Report>> ViewDistrict()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "GD");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DATAREPORT";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;  
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetDashboardData(ABP.Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_filterid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterid);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MONTH_DATA";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetMonthlyDashboardData(ABP.Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "MONDATA");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_filterid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterid);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_filterno", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterno);
                dyParam.Add("P_filternoto", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filternoto);
                
                dyParam.Add("P_condition", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.condition);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_MONTH_DATA";
                var query = "USP_ABP_MONTH_DATA_COPY";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetMonthlyYearlyDashboardData(ABP.Domain.Report.Report Rpt)
        {
            try
            {
              
              
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "MONDATA");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_filterid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterid);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_filterno", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterno);
                dyParam.Add("P_filternoto", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filternoto);
                dyParam.Add("P_condition", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.condition);
             
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_MONTH_DATA";
                var query = "USP_ABP_MONTH_DATA_BLOCK_COPY";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetMonthlyYearlyDashboardData2(ABP.Domain.Report.Report Rpt)
        {
            try
            {
                string Monthsel = "";
                string Monthsel2 = "";
                string Monthsel3 = "";
                if (Rpt.DisplayName != null)
                {
                    Monthsel = Rpt.DisplayName;
                    Monthsel2 = Monthsel.Remove(Monthsel.Length - 2);
                    Monthsel3 = Monthsel2.Replace(" ", "");
                }
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "MONDATA");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_filterid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterid);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_filterno", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterno);
                dyParam.Add("P_filternoto", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filternoto);
                dyParam.Add("P_condition", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.condition);
                dyParam.Add("P_month", OracleDbType.Varchar2, ParameterDirection.Input, Monthsel3);
                dyParam.Add("P_asprblock", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Aspirationblock);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_MONTH_DATA";

                var query = "USP_ABP_MONTH_DATA_BLOCK_COPY1";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetIndicatorData(ABP.Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_filterid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterid);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_INDICATORID", OracleDbType.Int32, ParameterDirection.Input, Rpt.IndicatorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORVALUE";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetIndicatorYearlyData(ABP.Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_filterid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterid);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_INDICATORID", OracleDbType.Int32, ParameterDirection.Input, Rpt.IndicatorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_INDICATORVALUEYEARLY";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetLogReport(ABP.Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LOG_REPORT";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetControlsByPanelid(int SectorId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_PANELID", OracleDbType.Varchar2, ParameterDirection.Input, SectorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_GETCONTROLSBYID";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.Report>> GetReportDashboardData(ABP.Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MRB");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_REPORT_DASHBOARD";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.Report>> GetMonthwiseReportData(Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MRD");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_REPORT_DASHBOARD";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<IEnumerable<Domain.Report.Report>> GetAnnualWiseReportData(Domain.Report.Report Rpt)
        //{
        //    try
        //    {
        //        var dyParam = new OracleDynamicParameters();
        //        dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
        //        dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
        //        dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
        //        dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
        //        var query = "USP_ABP_ANNUAL_DATA";
        //        var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public async Task<IEnumerable<Domain.Report.Report>> GetAnnualWiseReportData(Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_ANNUAL_DATA";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Report.Report>> GetYearlyWiseReportData(Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "ANNUALY");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_filterno", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterno);
                dyParam.Add("P_filternoto", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filternoto);
                dyParam.Add("P_condition", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.condition);
                dyParam.Add("P_asprblock", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Aspirationblock);

                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MONTH_DATA_COPY";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        public DataSet GetLogData(string APPLICATIONNO, string PanelName,int SectorId)
        {
            try
            {
                //DataSet ds = new DataSet();
                //DataTable dt = new DataTable();
                //DataTable Logdt = new DataTable();
                //var logtablename = PanelName + "_LOG";
                //using (OracleConnection conn = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                //{
                //    conn.Open();
                //    using (OracleCommand cmd = new OracleCommand("select ControlID,ControlName,DisplayName from  M_ABP_CONTROLPANEL where panelid='" + SectorId + "'", conn))
                //    {
                //        using (OracleDataReader reader = cmd.ExecuteReader())
                //        {
                //            dt.Load(reader);
                //        }
                //    }
                //}
                //using (OracleConnection conn2 = new OracleConnection(Configuration.GetValue<string>("MySettings:Con")))
                //{
                //    conn2.Open();
                //    using (OracleCommand cmd2 = new OracleCommand("select * from T_ABP_" + logtablename + " where ApplicationNo = '" + APPLICATIONNO + "'"))
                //    {
                //        using (OracleDataReader reader2 = cmd2.ExecuteReader())
                //        {
                //            Logdt.Load(reader2);
                //        }
                //    }
                //}
                //ds.Tables.Add(dt);
                //ds.Tables.Add(Logdt);
                //return ds;
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataTable Logdt = new DataTable();
                var logtablename = PanelName + "_LOG";
                var dyParam = new OracleDynamicParameters();
                //string Data = "SELECT pvch_control_name,trim(substring(substring(pvch_label_name, '[(][A-Z]+[)]', ''), '[^a-zA-Z ]', '')) as pvch_label_name,int_id FROM DF_FORM_CONFIG where int_form_id=" + Id + " and pvch_label_name is not null order by int_id";
                string Data = "select ControlID,ControlName,DisplayName from  M_ABP_CONTROLPANEL where panelid='" + SectorId + "'";
                IDataReader dr = Connection.ExecuteReader(Data);
                dt.Load(dr);
                string LogData = "select * from T_ABP_" + logtablename + " where ApplicationNo = '" + APPLICATIONNO + "'";
                IDataReader Ldr = Connection.ExecuteReader(LogData);
                Logdt.Load(Ldr);

                ds.Tables.Add(dt);
                ds.Tables.Add(Logdt);
                return ds;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Domain.Report.MapData>> GetDistrictWiseReport(Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.action);
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_Month", OracleDbType.Int32, ParameterDirection.Input, Rpt.FreqNo);
                //dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.MapData>> GetBlockByDistrictReport(Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MAPB");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_Month", OracleDbType.Int32, ParameterDirection.Input, Rpt.FreqNo);
                dyParam.Add("P_DistrictId", OracleDbType.Int32, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<MapData>> GetMapDatas(int id,int year, int month)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MSVG");
                dyParam.Add("P_svgid", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input,year);
                dyParam.Add("P_Month", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA_INFO";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<MapData>> GetDistrictDatas(int id, int year, int month)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DSB");
                dyParam.Add("P_svgid", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_Month", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA_INFO";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<MapData>> GetMapDatascore(int id, string year, int month)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MSD");
                dyParam.Add("P_svgid", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, year);
                dyParam.Add("P_Month", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA_INFO";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<BlockMap>> GetBlockMapData(int id)
        {   
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SV");
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA_INFO";
                var result = await Connection.QueryAsync<ABP.Domain.BlockMap.BlockMap>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MapData> GetDistMapDatas(int id)
        {
            try
            {
                MapData mp = new MapData();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DSVG");
                dyParam.Add("P_svgid", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA_INFO";
                mp = await Connection.QueryFirstOrDefaultAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return mp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.Report>> GetBlockMapDatas(int distid, int year, int freqno)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, distid);
                dyParam.Add("P_FREQNO", OracleDbType.Int32, ParameterDirection.Input, freqno);
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, year);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICT_MAP_INFO";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.Report>> GetDashboardDPData(Domain.Report.Report Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_blockid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.BlockId);
                dyParam.Add("P_filterid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.filterid);
                dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DATAPOINT_VIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Domain.Report.Report> GetApplicationNO(int blockid, int year, int freqno, int freqid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "Month");
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input,year);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, freqno);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, freqid);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETAPPNO";
                var result = await Connection.QueryFirstOrDefaultAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.BlockDpData>> GetBlockDatapoint(int blockid, string Appno)
        {
            try
            {


                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BDOSEL");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_APPLICATIONNO", OracleDbType.Int32, ParameterDirection.Input, Appno);

                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DPMONYEAR";
                var result = await Connection.QueryAsync<ABP.Domain.Report.BlockDpData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Domain.Report.Report> GetApplicationNOyearly(int blockid, int year, int freqid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "year");
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);               
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, freqid);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETAPPNO";
                var result = await Connection.QueryFirstOrDefaultAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Report.AbstractData>> GetDashboarAbstractdData(ABP.Domain.Report.AbstractData Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
               
                dyParam.Add("P_distid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.DistrictId);
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_category", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Aspirationblock);                
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                 //var query = "USP_ABP_ABSTRACT_DATA";
                var query = "USP_ABP_MONABSTRACTDATA";
                var result = await Connection.QueryAsync<ABP.Domain.Report.AbstractData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Report.SectorPRFData>> GetSectorEntryPrfData(Domain.Report.SectorPRFData Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SECTDATA");
                dyParam.Add("P_DISTID", OracleDbType.Int32, ParameterDirection.Input, Rpt.DISTRICTID);
                dyParam.Add("P_YEAR", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Rpt.BLOCKID);

                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_ABSTRACT_DATA";
                var query = "USP_ABP_SECTORDATAVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Report.SectorPRFData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ABP.Domain.Report.SectorPRFData>> GetSectorEntryTotalPrfData(Domain.Report.SectorPRFData Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SECTOTAL");
                dyParam.Add("P_DISTID", OracleDbType.Int32, ParameterDirection.Input, Rpt.DISTRICTID);
                dyParam.Add("P_YEAR", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Rpt.BLOCKID);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_ABSTRACT_DATA";
                var query = "USP_ABP_SECTORDATAVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Report.SectorPRFData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.SectorPRFData>> GetSectorEntryYearTotal(Domain.Report.SectorPRFData Rpt)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SECTTOTDATA");
                dyParam.Add("P_DISTID", OracleDbType.Int32, ParameterDirection.Input, Rpt.DISTRICTID);
                dyParam.Add("P_YEAR", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.Year);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Rpt.BLOCKID);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_ABSTRACT_DATA";
                var query = "USP_ABP_SECTORDATAVIEW";
                var result = await Connection.QueryAsync<ABP.Domain.Report.SectorPRFData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Domain.Report.Report> ApplicationNOyearly(int blockid, int year, int freqid, int monthno)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "year1");
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_FrequencyId", OracleDbType.Int32, ParameterDirection.Input, freqid);
                dyParam.Add("P_FREQUENCYNO", OracleDbType.Int32, ParameterDirection.Input, monthno);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETAPPNO";
                var result = await Connection.QueryFirstOrDefaultAsync<ABP.Domain.Report.Report>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetDpcount()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DPCNT");
        
                var query = "USP_ABP_BDO_ALERT";
                var result = Connection.QueryFirstOrDefault<string>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.MapData>> GetDistrictwiseMapData(int year,int freqno)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DMDATA");
                dyParam.Add("P_Year", OracleDbType.Varchar2, ParameterDirection.Input, year);//Rpt.Year
                dyParam.Add("P_Month", OracleDbType.Varchar2, ParameterDirection.Input, freqno);//Rpt.DistrictId
                //dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_NEWMAP_DATA";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.MapData>> GetEnterednotenteredData(int year, int freqno)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SWMAP");
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, year);//Rpt.Year
                dyParam.Add("P_Month", OracleDbType.Int32, ParameterDirection.Input, freqno);//Rpt.DistrictId
                //dyParam.Add("P_sectorid", OracleDbType.Varchar2, ParameterDirection.Input, Rpt.SectorId);
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA_NEW";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.Report.MapData>> GetEnterednotenteredDatadistwise(int year, int freqno,int distid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DISTMAPCNT");
                dyParam.Add("P_Year", OracleDbType.Int32, ParameterDirection.Input, year);//Rpt.Year
                dyParam.Add("P_Month", OracleDbType.Int32, ParameterDirection.Input, freqno);//Rpt.Month
                dyParam.Add("P_distid", OracleDbType.Int32, ParameterDirection.Input, distid);//Rpt.DistrictId
                dyParam.Add("P_result", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_MAP_DATA_NEW";
                var result = await Connection.QueryAsync<ABP.Domain.Report.MapData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
