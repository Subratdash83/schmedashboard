using ABP.Domain.Communication;
using ABP.Repository;
using ABP.Repository.Contract.Contract.SendMessage;
using ABP.Repository.Contract.Factory;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.SendMessage
{
  public  class MessageRepository : RepositoryBase, IMessageRepository
    {
        /// <summary>
        /// Defining the private readonly Logging instance
        /// </summary>
        private readonly ILogger<MessageRepository> Logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">Connection Factory</param>
        /// See also <see cref="FTP.Repository.Contract.Factory.IConnectionFactory" />
        public MessageRepository(IConnectionFactory connectionFactory, ILogger<MessageRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("SentMessageRepository initialized");
        }
        public async Task<COMMUNICATIONINBOX> BindInbox(CommunicationWrite Model)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "SENTMESSAGE");
                dyParam.Add("P_EMAILID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_DEPTID", OracleDbType.Int32, ParameterDirection.Input, Model.DEPTID);
                dyParam.Add("P_TYPE", OracleDbType.Varchar2, ParameterDirection.Input, Model.ACTION);        
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("Inbox_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_5T_BINDSENTITEM";
                var result = await Connection.QueryMultipleAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                var I1 = await result.ReadAsync<CommunicationWrite>();
                var I2 = await result.ReadAsync<COMMUNICATIONVIEW>();
                COMMUNICATIONINBOX viewModel = new COMMUNICATIONINBOX();
                viewModel.ViewInbox = I1.AsList();
                viewModel.TMAILINBOX = I2.AsList();
                return viewModel;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, Model);
                throw ex;
            }
        }
        public async Task<COMMUNICATIONINBOX> BindUnRead(CommunicationWrite Model)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "SENTUM");
                dyParam.Add("P_EMAILID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_DEPTID", OracleDbType.Int32, ParameterDirection.Input, Model.DEPTID);
                dyParam.Add("P_TYPE", OracleDbType.Varchar2, ParameterDirection.Input, Model.ACTION);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("Inbox_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_5T_BINDSENTITEM";                
                var result = await Connection.QueryMultipleAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                var I1 = await result.ReadAsync<CommunicationWrite>();
                var I2 = await result.ReadAsync<COMMUNICATIONVIEW>();
                COMMUNICATIONINBOX viewModel = new COMMUNICATIONINBOX();
                viewModel.ViewInbox = I1.AsList();
                viewModel.TMAILINBOX = I2.AsList();
                return viewModel;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<COMMUNICATIONINBOX> BindReadMessage(CommunicationWrite Model)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "SENTRM");
                dyParam.Add("P_EMAILID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_DEPTID", OracleDbType.Int32, ParameterDirection.Input, Model.DEPTID);
                dyParam.Add("P_TYPE", OracleDbType.Varchar2, ParameterDirection.Input, Model.ACTION);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("Inbox_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_5T_BINDSENTITEM";
                var result = await Connection.QueryMultipleAsync(query, dyParam, commandType: CommandType.StoredProcedure);
                var I1 = await result.ReadAsync<CommunicationWrite>();
                var I2 = await result.ReadAsync<COMMUNICATIONVIEW>();
                COMMUNICATIONINBOX viewModel = new COMMUNICATIONINBOX();
                viewModel.ViewInbox = I1.AsList();
                viewModel.TMAILINBOX = I2.AsList();
                return viewModel;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }


        public async Task<IEnumerable<CommunicationWrite>> ViewSendMessage(string EMAILID,int Deptid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "CHILDSENTMESSAGE");
                dyParam.Add("P_EMAILID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(EMAILID));
                dyParam.Add("P_DEPTID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(Deptid));
                dyParam.Add("P_TYPE", OracleDbType.Varchar2, ParameterDirection.Input, "");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("Inbox_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_5T_BINDSENTITEM";                               
                var result = await Connection.QueryAsync<CommunicationWrite>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        //public async Task<COMMUNICATIONINBOX> ViewSendMessage(Department dept)
        //{
        //    try
        //    {
        //        var dyParam = new OracleDynamicParameters();
        //        dyParam.Add("P_EMAILID", OracleDbType.Int32, ParameterDirection.Input, Convert.ToInt32(dept.EMAILID));
        //        dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "SENTMESSAGE");
        //        dyParam.Add("P_DEPTID", OracleDbType.Int32, ParameterDirection.Input, dept.DEPTID);
        //        dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
        //        dyParam.Add("Inbox_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
        //        var query = "USP_BINDINBOX_DETAILS";
        //        var result = await Connection.QueryMultipleAsync(query, dyParam, commandType: CommandType.StoredProcedure);
        //        var I1 = await result.ReadAsync<CommunicationWrite>();
        //        var I2 = await result.ReadAsync<COMMUNICATIONVIEW>();
        //        COMMUNICATIONINBOX viewModel = new COMMUNICATIONINBOX();
        //        viewModel.ViewInbox = I1.AsList();
        //        viewModel.TMAILINBOX = I2.AsList();
        //        return viewModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, ex.Message, dept);
        //        throw ex;
        //    }
        //}
    }
}
