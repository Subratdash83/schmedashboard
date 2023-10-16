using ABP.Domain.Communication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.SendMessage
{
  public  interface IMessageRepository
    {
        /// <summary>
        /// Bind Send message
        /// </summary>
        /// <param name="Inbox">Inbox parameter</param>
        /// <returns><seealso cref="FTP.Domain.Common.Department"/></returns>
        //Task<IEnumerable<CommunicationWrite>> BindInbox(CommunicationWrite Model);

        Task<COMMUNICATIONINBOX> BindInbox(CommunicationWrite Model);
        Task<IEnumerable<CommunicationWrite>> ViewSendMessage(string EMAILID,int Deptid);

        Task<COMMUNICATIONINBOX> BindUnRead(CommunicationWrite Model);
        /// <summary>
        /// Bind UnRead Message
        /// </summary>
        /// <param name="Inbox">Inbox parameter</param>
        /// 
        Task<COMMUNICATIONINBOX> BindReadMessage(CommunicationWrite Model);
        /// <summary>
        /// Bind Read Message
        /// </summary>
        /// <param name="Inbox">Inbox parameter</param>
    }
}
