//***********************************************************************
// Assembly         : FTP.Repository
// Author           : 
// Created          : 
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="CommonRepository.cs" company="CSM technologies">
//     Copyright ©  2020
// </copyright>
// <summary></summary>
// ***********************************************************************

using Dapper;
using Oracle.ManagedDataAccess.Client;
using ABP.Repository.Contract.Factory;
using ABP.Repository.Contract.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;


namespace ABP.Repository.Common
{
    /// <summary>
    /// Common Repository
    /// </summary>
    /// See also <see cref="FTP.Repository.RepositoryBase" />, <see cref="FTP.Repository.Common.CommonRepository" />
    public class CommonRepository : RepositoryBase, ICommonRepository
    {
        /// <summary>
        /// Defining the private readonly Logging instance
        /// </summary>
        private readonly ILogger<CommonRepository> Logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">Connection Factory</param>
        /// See also <see cref="FTP.Repository.Contract.Factory.IConnectionFactory" />
        public CommonRepository(IConnectionFactory connectionFactory, ILogger<CommonRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("CommonRepository initialized");
        }

        public async Task<IEnumerable<int>> UserAccessCheck(int PLink, int desigid, int userid, int check)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                if (check == 0)
                {
                    dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "D");
                }
                else
                {
                    dyParam.Add("P_Action", OracleDbType.Varchar2, ParameterDirection.Input, "U");
                }

                dyParam.Add("P_PLINKID", OracleDbType.Int32, ParameterDirection.Input, PLink);
                dyParam.Add("P_Desigid", OracleDbType.Int32, ParameterDirection.Input, desigid);
                dyParam.Add("P_Userid", OracleDbType.Int32, ParameterDirection.Input, userid);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                
                //p.Add("P_Msg", MySqlDbType.VarChar, direction: ParameterDirection.Output);
                var query = "USP_USER_AUTHENTICATION";
                var userId = await Connection.QueryAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);
                return userId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }




    }
}
