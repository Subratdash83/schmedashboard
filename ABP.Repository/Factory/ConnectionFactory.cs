// ***********************************************************************
// Assembly         : ABP.Repository
// Author           : Monalisa Pahi
// Created          : 15-MAR-2021
//
// Last Modified By : Monalisa Pahi
// Last Modified On : 15-MAR-2021
// ***********************************************************************
// <copyright file="ConnectionFactory.cs" company="CSM technologies">
//     Copyright ©  2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABP.Repository.Contract.Factory;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ABP.Repository.Factory
{
    /// <summary>
    /// Class ConnectionFactory.
    /// </summary>
    /// <seealso cref="ABP.Repository.Contract.Factory.IConnectionFactory" />
    public class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets the get connection.
        /// </summary>
        /// <value>The get connection.</value>
        public IDbConnection GetConnection => new OracleConnection(_connectionString);
    }
}
