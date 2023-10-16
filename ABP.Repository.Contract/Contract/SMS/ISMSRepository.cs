using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.SMS
{
    public interface ISMSRepository:IDisposable,IRepository
    {
        Task<IEnumerable<ABP.Domain.SMS.SMS>> GetMobilenumber();
        List<ABP.Domain.SMS.SMS> GetUserDetails();
        ABP.Domain.SMS.SMS GetBDODATA(ABP.Domain.SMS.SMS sMS);

        void TrackSMS(int INTUSERID, string TEMPLATENAME, string smsresult);
    }
}
