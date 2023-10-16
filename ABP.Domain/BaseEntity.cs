// ***********************************************************************
// Assembly         : FTP.Domain
// Author           : Manoj Kumar Behera
// Created          : 31-JUL-2019
//
// Last Modified By : Manoj Kumar Behera
// Last Modified On : 
// ***********************************************************************
// <copyright file="BaseEntity.cs" company="CSM technologies">
//     Copyright ©  2019
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ABP.Domain
{
    /// <summary>
    /// Interface IEntity
    /// </summary>
    public interface IEntity
    {
    }
    /// <summary>
    /// Class BaseEntity.
    /// </summary>
    /// <seealso cref="FTP.Domain.IEntity" />
    public abstract class BaseEntity : IEntity
    {
    }
    /// <summary>
    /// Class BaseEntity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="FTP.Domain.BaseEntity" />
    /// <seealso cref="FTP.Domain.IEntity" />
    public abstract class BaseEntity<T> : BaseEntity, IEntity
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public abstract T Key { get; set; }
    }

}
