using ABP.Domain;
using ABP.Repository;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.DistDetailReport;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.DistDetailReport
{
   public class DistDetailRepository: RepositoryBase, IDistDetailRepository
    {
        private readonly ILogger<DistDetailRepository> Logger;
        public DistDetailRepository(IConnectionFactory connectionFactory, ILogger<DistDetailRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("DistrictRepository initialized");
        }

        public List<Domain.Report.DistReport> GetAllDistrictUserAsync()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GETALLDISTRICTUSER");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_INFO";
                var result = Connection.Query<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure).AsList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }


        public async Task<IEnumerable<Domain.Report.DistReport>> GetBlockByDistrict(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "SGB");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Report.DistReport>> GetByDistrictDataDetails(int DistCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "GUS");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistCode));
               //dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(BlockCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Report.DistReport>(query, dyParam, commandType: CommandType.StoredProcedure);
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
