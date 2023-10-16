using ABP.Domain;
using ABP.Repository.Contract.Contract.AdminConsole;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.AdminConsole
{
    public class AdminConsoleRepository : RepositoryBase, IAdminConsoleRepository
    {
        protected readonly ILogger<AdminConsoleRepository> Logger;
        public AdminConsoleRepository(IConnectionFactory connectionFactory, ILogger<AdminConsoleRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("AdminConsoleRepository initialized");
        }

        public AdminConsoleRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            LoggerFactory logfac = new LoggerFactory();
            Logger = logfac.CreateLogger<AdminConsoleRepository>();
        }
        public async Task<int> GetUserId(string VCHUSERNAME)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            try
            {
                string sql = "select INTUSERID from M_POR_USER where upper(VCHUSERNAME) ='" + VCHUSERNAME.ToUpper() + "'";
                var userId = await Connection.QueryFirstAsync<int>(sql);
                return userId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<LinkAccess> GetLinkAccessByUserId(int UserId, int ProjectId, int desgid)
        {

            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            try
            {
                var aParam = new OracleDynamicParameters();
                aParam.Add("p_action", OracleDbType.Char, ParameterDirection.Input, "VU");
                aParam.Add("p_intintuserid", OracleDbType.Int32, ParameterDirection.Input, UserId);
                aParam.Add("p_intprojectid", OracleDbType.Int32, ParameterDirection.Input, ProjectId);
                aParam.Add("p_intdesignationid", OracleDbType.Int32, ParameterDirection.Input, desgid);
                aParam.Add("cur", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_USERMASTER_VIEW";
                var result = Connection.Query<LinkAccess>(query, aParam, commandType: CommandType.StoredProcedure);
                return result.ToList();

            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw exception;
                //throw new Exception("Execution Failed", exception);
            }
        }
    }
}
