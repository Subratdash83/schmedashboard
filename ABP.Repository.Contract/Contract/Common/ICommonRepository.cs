// ***********************************************************************
// Assembly         : FTP.Repository.Contract
// Author           : 
// Created          : 09-SEP-2019
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="IRepository.cs" company="CSM technologies">
//     Copyright ©  2019
// </copyright>
// <summary></summary>
// ***********************************************************************


using ABP.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Common
{

    /// <summary>
    /// Interface ICommonRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface ICommonRepository : IDisposable, IRepository
    {
        /// <summary>
        /// Get States
        /// </summary>
        /// <returns><seealso cref="FTP.Domain.Common.States"/></returns>
        Task<IEnumerable<int>> UserAccessCheck(int PLink, int desigid, int userid, int check);
    }
}
