using ABP.Domain;
using ABP.Domain.Login;
using ABP.Domain.Login.OTP;
using ABP.Persistence;
using ABP.Repository.Contract.Contract.Login;
using ABP.Repository.Contract.Factory;
using ABP.Repository.Factory;
using ClientsideEncryption;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;
using static System.Net.WebRequestMethods;

namespace ABP.Repository.Login
{
    /// <summary>
    /// Login Repository
    /// </summary>
    /// See also <see cref="FTP.Repository.RepositoryBase" />, <see cref="FTP.Repository.Login.LoginRepository" />
    public class LoginRepository : RepositoryBase, ILoginRepository
    {

        /// <summary>
        /// Defining the private readonly Logging instance
        /// </summary>
        private readonly ILogger<LoginRepository> Logger;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">Connection Factory</param>
        /// See also <see cref="FTP.Repository.Contract.Factory.IConnectionFactory" />
        public LoginRepository(IConnectionFactory connectionFactory, ILogger<LoginRepository> logger, IConfiguration configuration) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("LoginRepository initialized");
            _configuration = configuration;
        }

        /// <summary>
        /// Get Login Details
        /// </summary>
        public async Task<IEnumerable<Domain.Login.Login>> GetLoginDetails(Domain.Login.Login logins)
        {

            try
            {
                string ss = CommonFunction.DecryptData("B3DWFLG5tJjUPfQfisZGqg==");

                // string ss = CommonFunction.DecryptData("sIm/Nfv8vyQ91XgFrB9nzQ==");
                // string ss = CommonFunction.DecryptData("kQhkhFsRWLJxnz7yArdmMQ==");
                //string ss = CommonFunction.DecryptData("QLZ5bOEN8e3UR5be1LN5tw==");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, logins.VCHUSERNAME);
                dyParam.Add("P_PASSWORD", OracleDbType.Varchar2, ParameterDirection.Input, CommonFunction.EncryptData(logins.vchpassword));
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "Login");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, logins);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Login.Login>> GetMLoginDetails(Domain.Login.Login logins)
        {

            try
            {
                string ss = CommonFunction.DecryptData("vQekEpA+nKoBPey1aI9Yrw==");

                // string ss = CommonFunction.DecryptData("sIm/Nfv8vyQ91XgFrB9nzQ==");
                // string ss = CommonFunction.DecryptData("kQhkhFsRWLJxnz7yArdmMQ==");
                //string ss = CommonFunction.DecryptData("QLZ5bOEN8e3UR5be1LN5tw==");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, logins.VCHUSERNAME);
                //dyParam.Add("P_PASSWORD", OracleDbType.Varchar2, ParameterDirection.Input, CommonFunction.EncryptData(logins.vchpassword));
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "LM");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, logins);
                throw ex;
            }
        }


        public async Task<IEnumerable<Domain.Login.Login>> GetLoginDetailsLDAP(Domain.Login.Login logins)
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
                //dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, CommonFunction.DecryptData(logins.VCHUSERNAME));
                dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, logins.VCHUSERNAME);
                //dyParam.Add("P_PASSWORD", OracleDbType.Varchar2, ParameterDirection.Input, CommonFunction.EncryptData(logins.vchpassword));
                //dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "Login");  
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "L_Ldap");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, logins);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Login.Login>> GetLoginDetailsApi(Domain.Login.Login logins)
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, logins.VCHUSERNAME);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "L_Api");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, logins);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Login.Login>> InsertLoginDetails(int UserId)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "I");
                dyParam.Add("P_USERID", OracleDbType.Int32, ParameterDirection.Input, UserId);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Login.Login>> UpdateLoginDetails(int? logid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "U");
                dyParam.Add("P_LOGID", OracleDbType.Int32, ParameterDirection.Input, logid);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Login.Login>> GetInsertDetails(Domain.Login.Login logins)
        {

            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "ID");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, logins);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.Login.Login>> GetLoginDetailsByUserId(Domain.Login.Login logins)
        {

            try
            {
                string ss = CommonFunction.DecryptData("vQekEpA+nKoBPey1aI9Yrw==");
                // string ss = CommonFunction.DecryptData("u8dE5Ly7NbuQsDpsCmrIbA==");
                //; string ss = CommonFunction.DecryptData("kQhkhFsRWLJxnz7yArdmMQ==");
                var dyParam = new OracleDynamicParameters();
                string spcid = string.Empty;
                string HRMSUSERNAME = string.Empty;
                string VCHUSERNAME = string.Empty;
                string MOBILENO = string.Empty;
                if (logins.vchspcid == null)
                {
                    spcid = "0";
                }
                else
                {
                    spcid = logins.vchspcid;
                    MOBILENO = logins.MOBILENO;
                }
                if (logins.HRMSUSERNAME == null)
                {
                    HRMSUSERNAME = "0";
                }
                else
                {
                    HRMSUSERNAME = logins.HRMSUSERNAME;
                    MOBILENO = logins.MOBILENO;
                }
                if (logins.VCHUSERNAME == null)
                {
                    VCHUSERNAME = "0";
                }
                else
                {
                    VCHUSERNAME = logins.VCHUSERNAME;
                    MOBILENO = logins.MOBILENO;
                }
                dyParam.Add("P_SPCID", OracleDbType.Varchar2, ParameterDirection.Input, spcid);
                dyParam.Add("P_INTUSERID", OracleDbType.Int32, ParameterDirection.Input, logins.INTUSERID);
                dyParam.Add("P_INTLEVELDETAILID", OracleDbType.Int32, ParameterDirection.Input, logins.INTLEVELDETAILID);
                dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, VCHUSERNAME);
                dyParam.Add("P_HRMSUSERNAME", OracleDbType.Varchar2, ParameterDirection.Input, HRMSUSERNAME);
                dyParam.Add("P_MOBILENO", OracleDbType.Varchar2, ParameterDirection.Input, MOBILENO);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BYID");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_USER_LOGIN";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, logins);
                throw ex;
            }
        }

        public string ForgetPasswordNew(string UserName, string Password)
        {
            string strOutput = "";
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "CP");
                //dyParam.Add("P_XML_DATA", OracleDbType.XmlType, ParameterDirection.Input, UserName);
                dyParam.Add("P_PASSWORD", OracleDbType.Varchar2, ParameterDirection.Input, CommonFunction.EncryptData(Password));
                dyParam.Add("P_USERNAME", OracleDbType.Varchar2, ParameterDirection.Input, UserName);
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, direction: ParameterDirection.Output);
                var query = "USP_ABP_USER_LOGIN";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                strOutput = SuccessMessages.AsList()[0].successid.ToString();
                return strOutput;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<IEnumerable<Domain.Login.Login>> InsertLoginlogDetails(int userid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_USERID", OracleDbType.Varchar2, ParameterDirection.Input, userid);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "ILL");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LOGIN_LOG_MANAGE";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, userid);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Login.Login>> UpdateLoginlogDetails(int userid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_USERID", OracleDbType.Varchar2, ParameterDirection.Input, userid);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "ULL");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LOGIN_LOG_MANAGE";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, userid);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Login.Login>> GetLoginLogDetails(string fromdate, string todate)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_FROMDATE", OracleDbType.Varchar2, ParameterDirection.Input, fromdate);
                dyParam.Add("P_TODATE", OracleDbType.Varchar2, ParameterDirection.Input, todate);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "RLL");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LOGIN_LOG_MANAGE";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public List<Domain.Login.Login> GetAllBlockUserAsync()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GETALLBLOCKUSER");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_INFO";
                var result = Connection.Query<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure).AsList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public List<Domain.Login.Login> GetAllDistrictUserAsync()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();

                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GETALLDISTRICTUSER");
                dyParam.Add("P_Msg", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_SMS_INFO";
                var result = Connection.Query<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure).AsList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Login.Login>> GetBlockEnteredDetails(string fromdate, string todate, string status)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_FROMDATE", OracleDbType.Varchar2, ParameterDirection.Input, fromdate);
                dyParam.Add("P_TODATE", OracleDbType.Varchar2, ParameterDirection.Input, todate);
                dyParam.Add("P_STATUS", OracleDbType.Varchar2, ParameterDirection.Input, status);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "ecc");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_LOGIN_LOG_MANAGE";
                var result = await Connection.QueryAsync<Domain.Login.Login>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }


        public async Task<IEnumerable<Domain.BlockData.BlockDataAlert>> CountNotification(int Blockid)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "BDO");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, Blockid);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_NOTIFICATIONCOUNT";
                var result = await Connection.QueryAsync<Domain.BlockData.BlockDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.BlockData.BlockDataAlert>> CountNotificationCol(int Distid)
        {
            try
            {

                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "COLL");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, Distid);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_NOTIFICATIONCOUNT";
                var result = await Connection.QueryAsync<Domain.BlockData.BlockDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        public async Task<IEnumerable<Domain.BlockData.BlockDataAlert>> CountNotificationDept()
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "DEPT");
                dyParam.Add("P_BLOCKID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_DISTRICTID", OracleDbType.Int32, ParameterDirection.Input, 0);
                dyParam.Add("P_MSG", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_ABP_NOTIFICATIONCOUNT";
                var result = await Connection.QueryAsync<Domain.BlockData.BlockDataAlert>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        #region Forget password repository
        public async Task<ForgetPassword> ForgetPasswordNew(ForgetPassword forgetPassword)
        {
            try
            {

                var dyParam = new OracleDynamicParameters();
                dyParam.Add("P_UserName", OracleDbType.Varchar2, ParameterDirection.Input, forgetPassword.UserName);
                dyParam.Add("P_PhoneNumber", OracleDbType.Varchar2, ParameterDirection.Input, forgetPassword.MobileNumber);
                dyParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "INSERT");
                dyParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_OTP", OracleDbType.Varchar2, ParameterDirection.Input, forgetPassword.OTP);
                dyParam.Add("P_Tokens", OracleDbType.Varchar2, ParameterDirection.Input, forgetPassword.Token);

                var query = "USP_INSERT_OTP_REGISTRATION";
                //var result = await Connection.QueryAsync<ForgetPassword>(query, dyParam, commandType: CommandType.StoredProcedure);
                var result = await Connection.QuerySingleOrDefaultAsync<ForgetPassword>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                var errorResult = new ForgetPassword
                {
                    Message = ex.Message
                };
                return errorResult;
            }
        }
        public async Task<ResetPassword> ChangePassword(ResetPassword resetPassword)
        {
            try
            {
                //string ss = CommonFunction.DecryptData("6lLet7eoCMKSdYbvVTf9qA==");
                var dynParam = new OracleDynamicParameters();
                dynParam.Add("P_UserName", OracleDbType.Varchar2, ParameterDirection.Input, resetPassword.UserName);
                dynParam.Add("P_PhoneNumber", OracleDbType.Varchar2, ParameterDirection.Input, resetPassword.MobileNumber);
                dynParam.Add("P_PSWD", OracleDbType.Varchar2, ParameterDirection.Input, CommonFunction.EncryptData(resetPassword.NewPassword));
                dynParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                dynParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "ResetPswrd");
                var query = "USP_INSERT_OTP_REGISTRATION";
                var result = await Connection.QuerySingleOrDefaultAsync<ResetPassword>(query, dynParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> GetOTP(string Username, string PhoneNumber)
        {
            try
            {
                var dynParam = new OracleDynamicParameters();
                dynParam.Add("P_UserName", OracleDbType.Varchar2, ParameterDirection.Input, Username);
                dynParam.Add("P_PhoneNumber", OracleDbType.Varchar2, ParameterDirection.Input, PhoneNumber);
                dynParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "GetOTP");
                dynParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_INSERT_OTP_REGISTRATION";
                var result = await Connection.QueryAsync(query, param: dynParam, commandType: CommandType.StoredProcedure);
                var otp = ""; 

                foreach (var row in result)
                {otp = row.OTP?.ToString(); 
                    break; 
                }

                return otp;
            
            }
            catch (Exception)
            {
                throw; 
            }
        }

        public async Task<string> UpdateOTPStatus(string Username, string OTP, string PhoneNumber)
        {
            try
            {
                var dynParam = new OracleDynamicParameters();
                dynParam.Add("P_UserName", OracleDbType.Varchar2, ParameterDirection.Input, Username);
                dynParam.Add("P_PhoneNumber", OracleDbType.Varchar2, ParameterDirection.Input, PhoneNumber);
                dynParam.Add("P_OTP", OracleDbType.Varchar2, ParameterDirection.Input, OTP);
                dynParam.Add("P_ACTION", OracleDbType.Varchar2, ParameterDirection.Input, "IsVerify");
                dynParam.Add("EMP_DETAIL_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "USP_INSERT_OTP_REGISTRATION";

                int result = await Connection.QuerySingleOrDefaultAsync<int>(query, dynParam, commandType: CommandType.StoredProcedure);

                if (result == -1)
                {
                    throw new Exception("Stored procedure execution failed.");
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

    }
}
