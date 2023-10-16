using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.Department
{

    /// <summary>
    /// Interface IIndicatorRepository 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="FTP.Repository.Contract.IRepository" />
    public interface IDepartmentRepository
    {
        /// <summary>
        /// Get Department Detail
        /// </summary>
        /// <param name="ManageDepartment">Department ParaMeter</param>
        /// <returns><seealso cref="Domain.Department.Department"/></returns>     
        string AddDepartment(Domain.Department.Department dept);
        Task<IEnumerable<Domain.Department.Department>> ViewDepartment(int sectorid,int septid);
        string DeleteDepartment(Domain.Department.Department dept);
        Task<IEnumerable<Domain.Department.Department>> DepartmentGateById(int id);
        Task<IEnumerable<Domain.Department.Department>> ViewLevels();
       
    }
}
