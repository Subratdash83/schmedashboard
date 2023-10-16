using ABP.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.AdminConsole
{
    public interface IAdminConsoleRepository : IDisposable, IRepository
    {
        Task<int> GetUserId(string VCHUSERNAME);
        IList<LinkAccess> GetLinkAccessByUserId(int UserId, int ProjectId, int desgid);
    }
}
