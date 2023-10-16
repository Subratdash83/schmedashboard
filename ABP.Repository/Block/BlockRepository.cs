using ABP.Domain;
using ABP.Persistence;
using ABP.Repository.Contract.Block;
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

namespace ABP.Repository.Block
{

    /// <summary>
    /// BlockRepository Repository
    /// </summary>
    /// See also <see cref="FTP.Repository.RepositoryBase" />, <see cref="FTP.Repository.Login.LoginRepository" />

    public class BlockRepository : RepositoryBase, IBlockRepository
    {
        /// <summary>
        /// Defining the private readonly Logging instance
        /// </summary>
        private readonly ILogger<BlockRepository> Logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">Connection Factory</param>
        /// See also <see cref="FTP.Repository.Contract.Factory.IConnectionFactory" />
        /// 

        public BlockRepository(IConnectionFactory connectionFactory, ILogger<BlockRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("BlockRepository initialized");
        }
        public string InsertBlock(XElement Xml, int DistCode)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "I");
                p.Add("P_DISTCODE", OracleDbType.Int32, ParameterDirection.Input, DistCode);
                p.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, Xml);
                p.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                //p.Add("P_UPDATEBY", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(0));
                var query = "USP_ABP_ADDBLOCK";
                //SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                var result = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return result.AsList()[0].successid.ToString();
                //return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> GetBlockByDist(string ACTION, int DistId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, ACTION);
                dyParam.Add("PDISTCODE", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistId));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_GETBLOCKBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetMappedeBlockByDist(int DistId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "GET");
                dyParam.Add("PDISTCODE", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistId));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_GETMAPBLOCKBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string DeleteBlock(int BlockCode)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var p = new OracleDynamicParameters();
                p.Add("P_DISTID", OracleDbType.Int32, ParameterDirection.Input, BlockCode);
                p.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_DELETEBLOCK";
                SuccessMessages = Connection.Query<SuccessMessage>(query, p, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetBlockAsync()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETALLBLOCKS";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> GetBlockByDistId(int DistID)
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "GB");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, DistID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.CollectorData.BlockData>> GetBlockDistBlock(int DistID)
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Char, ParameterDirection.Input, "GBI");
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, DistID);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_BLOCKSIDBYDISTID";
                var result = await Connection.QueryAsync<Domain.CollectorData.BlockData>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.District.District>> GetBlockDetailsByID(int DostCode)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("PDISTCODE", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DostCode));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_GETALLBLOCKSSEARCH";
                var result = await Connection.QueryAsync<Domain.District.District>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> GetBlockByDistModal(int DistId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("PDISTCODE", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(DistId));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_GETALLBLOCKBYDISTMODAL";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> BlockView()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_BLOCKVIEW";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> GetBlockDetailsByBlockId(int BlockId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(BlockId));
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_BLOCKDETAILSBYBLOCKID";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetBlockCount(int levelid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BLOCKCOUNT");
                dyParam.Add("P_LEVELID", OracleDbType.Int32, ParameterDirection.Input, levelid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_BLOCKCOUNT";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Block.Block>> GetMapBlockCount(int levelid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "MAPBLOCKCOUNT");
                dyParam.Add("P_LEVELID", OracleDbType.Int32, ParameterDirection.Input, levelid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_BLOCKCOUNT";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Block.Block>> GetAutoMapBlockCount(int levelid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "AUTOMAPBLOCKCOUNT");
                dyParam.Add("P_LEVELID", OracleDbType.Int32, ParameterDirection.Input, levelid);
                dyParam.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_BLOCKCOUNT";
                var result = await Connection.QueryAsync<Domain.Block.Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

