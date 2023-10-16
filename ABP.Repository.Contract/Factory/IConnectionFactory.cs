// ***********************************************************************
// Assembly         : FTP.Repository.Contract
// Author           : Monalisa Pahi
// Created          : 15-MAR-2021
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="IConnectionFactory.cs" company="CSM technologies">
//     Copyright ©  2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Data;

namespace ABP.Repository.Contract.Factory
{
    /// <summary>
    /// Interface IConnectionFactory
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Gets the get connection.
        /// </summary>
        /// <value>The get connection.</value>
        IDbConnection GetConnection { get; }
    }
}
