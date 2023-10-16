using ABP.Domain;
using ABP.Domain.Panel;
using ABP.Domain.Sector;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Sector
{
    public class SectorRepository:RepositoryBase,ISectorRepository
    {
        private readonly ILogger<SectorRepository> Logger;
        public SectorRepository(IConnectionFactory connectionFactory, ILogger<SectorRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("SectorRepository initialized");
        }

        public string InsertSector(sector sector)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (sector.SECTORID != 0)
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "UPDATE");
                }
                else
                {
                    dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "INSERT");

                }
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, sector.SECTORID);
                dyParam.Add("P_Sectorname", OracleDbType.Varchar2, ParameterDirection.Input, sector.SECTORNAME);
                dyParam.Add("P_Date", OracleDbType.Varchar2, ParameterDirection.Input, sector.SDATE);
                dyParam.Add("P_Userid", OracleDbType.Int32, ParameterDirection.Input, sector.CREATEDBY);
                dyParam.Add("P_CSSCLASS", OracleDbType.Varchar2, ParameterDirection.Input, sector.CSSCLASS);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SECTORADDUPDATE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                //if (result.successid == 4)
                //{
                //    return "4";
                //}
                //else if (result.AsList()[0].successid == 5)
                //{
                //    return "5";
                //}
                //else if (result.AsList()[0].successid == 1)
                //{
                //    return "1";
                //}
                //else
                //{
                //    var strOutput = result.AsList()[0].successmessage;
                return strOutput;
                //}
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, sector);
                throw ex;
            }
            //return strOutput;

        }

        public async Task<IEnumerable<sector>> ViewSector()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "VIEW");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SECTORVIEWDELETE";
                var result = await Connection.QueryAsync<sector>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DeleteSector(sector Model)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "DELETE");
                p.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, Model.SECTORID);
                p.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SECTORVIEWDELETE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Sector.sector>> SectorGateById(int id)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_SectorId", OracleDbType.Int32, ParameterDirection.Input, id);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETSECTORBYID";
                var result = await Connection.QueryAsync<Domain.Sector.sector>(query, dyParam, commandType: CommandType.StoredProcedure);
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
