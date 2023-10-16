using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ABP.Repository.Contract.Contract.CollectorData
{
    /// <summary>
    /// Interface ICollectorDataRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface ICollectorDataRepository
    {
        /// <summary>
        /// Insert CollectorData Detail
        /// </summary>
        /// <param name="ManageCollectorData">CollectorData ParaMeter</param>
        /// <returns><seealso cref="Domain.CollectorData.CollectorData"/></returns>

        void AddCollectorData(Domain.DataPoint.DataPoint dp);
        string AddCollectorData(XElement xml, int STATUS, int userid,string Remark);
    }
}
