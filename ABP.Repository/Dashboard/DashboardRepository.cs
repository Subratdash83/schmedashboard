using ABP.Domain.Dashboard;
using ABP.Domain.DashboardModel;
using ABP.Repository.Contract.Contract.Dashboard;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Dashboard
{
   public class DashboardRepository : RepositoryBase, IDashboardRepository
    {
        /// <summary>
        /// Defining the private readonly Logging instance
        /// </summary>
        private readonly ILogger<DashboardRepository> Logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">Connection Factory</param>
        /// See also <see cref="FTP.Repository.Contract.Factory.IConnectionFactory" />
        public DashboardRepository(IConnectionFactory connectionFactory, ILogger<DashboardRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("DashboardRepository initialized");
        }
        public async Task<IEnumerable<DashboardModel>> GetDashboardData(int Distid, int Blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "V");
                dyParam.Add("P_DistId", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_BlockId", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DASHBOARD";
                var result = await Connection.QueryAsync<DashboardModel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardModel>> GetDashboardDetails(int SectorId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "S");
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, SectorId);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DASHBOARD";
                var result = await Connection.QueryAsync<DashboardModel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardModel>> GetCompositeData()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "A");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DASHBOARD";
                var result = await Connection.QueryAsync<DashboardModel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<ABP.Domain.DataPoint.DataPoint>> GetRejectedDataDetails(int Leveldetailid,int Status)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "RD");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Leveldetailid);
                dyParam.Add("P_STATUS", OracleDbType.Int32, ParameterDirection.Input, Status);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_DISTRICTDATA_MANAGE";
                var result = await Connection.QueryAsync<Domain.DataPoint.DataPoint>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardNew>> AllSectorIndicatorcounts()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "AI");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_BLOCK_DATAPOINTVIEW_AINDICNT";
                var query = "USP_ABP_BLOCK_AINDICNT"; 
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardNew>> AllSectorDatapointcounts(int month)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "ADPCNT");
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_BLOCK_DATAPOINTVIEW";
                var query = "USP_ABP_BLOCK_ADPCNT";
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardNew>> MonthlyDatapointcounts(int year, int month, int blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MONTHDP");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_BLOCK_DATAPOINTVIEW";
                var query = "USP_ABP_BLOCK_MONTHDP"; 
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardNew>> YearlyDatapointcounts(int year, int month, int blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "YEARLYDP");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_BLOCK_DATAPOINTVIEW";
                var query = "USP_ABP_BLOCK_YEARLYDP";
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardNew>> GetIndicatorBySector(int sectorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "IDPSECTOR");
                dyParam.Add("P_SECTORID", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_BLOCK_DATAPOINTVIEW";
                var query = "USP_ABP_BLOCK_IDPSECTOR"; 
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<DashboardNew>> GetBlockWiseDataByDistID(int year, int month, int distid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BLKENTERD");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, distid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTOR_DPVIEW";
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        } 
        public async Task<IEnumerable<DashboardNew>> GetSectorDataByBlockID(int year, int month, int blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "SECENTERED");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_COLLECTOR_DPVIEW";
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
        //Sectorwise Indicators and its Datapoint Entered or Not Entered
        public async Task<IEnumerable<DashboardNew>> GetIndicatorDPBySector(int year, int month, int blockid, int sectorid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INDDPEN");
                dyParam.Add("P_SECTORID", OracleDbType.Int32, ParameterDirection.Input, sectorid);
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, blockid);
                dyParam.Add("P_YEAR", OracleDbType.Int32, ParameterDirection.Input, year);
                dyParam.Add("P_MONTH", OracleDbType.Int32, ParameterDirection.Input, month);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                //var query = "USP_ABP_BLOCK_DATAPOINTVIEW";
                var query = "USP_ABP_BLOCK_INDDPEN";
                var result = await Connection.QueryAsync<DashboardNew>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
