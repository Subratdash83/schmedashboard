using ABP.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Infrastructure
{
    public interface ISendSMSRepository : IDisposable, IRepository
    {
        Task<string> Sendsms(string mobno, string content, string templateid);
        
    }
}
