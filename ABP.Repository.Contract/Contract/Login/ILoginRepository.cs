
// ***********************************************************************
// Assembly         : FTP.Repository.Contract
// Author           : 
// Created          : 12-FEB-2020
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="IRepository.cs" company="CSM technologies">
//     Copyright ©  2020
// </copyright>
// <summary></summary>
// ***********************************************************************


using ABP.Domain.Login.OTP;
using ABP.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.Login
{

    /// <summary>
    /// Interface ILoginRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface ILoginRepository : IDisposable, IRepository
    {

        /// <summary>
        /// Get Login Detail
        /// </summary>
        /// <param name="LoginClosure">Login Request ParaMeter</param>
        /// <returns><seealso cref="Domain.Login.Login"/></returns>
        /// 


        Task<IEnumerable<ABP.Domain.Login.Login>> GetLoginDetails(ABP.Domain.Login.Login logins);

        Task<IEnumerable<ABP.Domain.Login.Login>> GetMLoginDetails(ABP.Domain.Login.Login logins);
        Task<IEnumerable<ABP.Domain.Login.Login>> GetLoginDetailsLDAP(ABP.Domain.Login.Login logins);

        Task<IEnumerable<ABP.Domain.Login.Login>> InsertLoginDetails(int UserId);
        Task<IEnumerable<ABP.Domain.Login.Login>> UpdateLoginDetails(int? logid);

        Task<IEnumerable<ABP.Domain.Login.Login>> GetInsertDetails(ABP.Domain.Login.Login logins);
        Task<IEnumerable<ABP.Domain.Login.Login>> GetLoginDetailsApi(ABP.Domain.Login.Login logins);
        Task<IEnumerable<Domain.Login.Login>> GetLoginDetailsByUserId(Domain.Login.Login logins);

        //Task<IEnumerable<Domain.Login.Login>> GetRejectedData(int leveldetailid); 
        Task<IEnumerable<Domain.Login.Login>> InsertLoginlogDetails(int userid);
        Task<IEnumerable<Domain.Login.Login>> UpdateLoginlogDetails(int userid);
        Task<IEnumerable<Domain.Login.Login>> GetLoginLogDetails(string fromdate, string todate);
        Task<IEnumerable<Domain.Login.Login>> GetBlockEnteredDetails(string fromdate, string todate, string status);
        // Task<IEnumerable<Domain.Login.Login>> GetBlockAsync();
        List<Domain.Login.Login> GetAllBlockUserAsync();
        List<Domain.Login.Login> GetAllDistrictUserAsync();
        Task<IEnumerable<Domain.BlockData.BlockDataAlert>> CountNotification(int userid);
        Task<IEnumerable<Domain.BlockData.BlockDataAlert>> CountNotificationCol(int userid);
        Task<IEnumerable<Domain.BlockData.BlockDataAlert>> CountNotificationDept();

        Task<ForgetPassword> ForgetPasswordNew(ForgetPassword forgetPassword);
        Task<ResetPassword> ChangePassword(ResetPassword resetPassword);
        Task<string> UpdateOTPStatus(string Username, string OTP, string PhoneNumber);
        Task<string> GetOTP(string Username, string PhoneNumber);
    }
}
