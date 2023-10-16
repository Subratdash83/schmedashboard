using ABP.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ABP.Domain.Communication
{
   public class CommunicationWrite : BaseEntity<int>
    {
        /// <summary>
        /// EMAILID(Master Primary Key)
        /// </summary>
        public int EMAILID { get; set; }
        /// <summary>
        /// MAIL DATE
        /// </summary>
        public string MAILDATE { get; set; }
        /// <summary>
        /// FROM DEPARTMENT
        /// </summary>
        public int FROMDEPARTMENT { get; set; }
        /// <summary>
        /// TO DEPARTMENT
        /// </summary>
        public string TODEPARTMENT { get; set; }
        /// <summary>
        /// COMPONENT
        /// </summary>
        public string COMPONENT { get; set; }

        public string deptname { get; set; }
        /// <summary>
        /// MESSAGE
        /// </summary>
        public string MESSAGE { get; set; }
        /// <summary>
        /// FILENAME
        /// </summary>
        public string FILENAME { get; set; }
        /// <summary>
        /// READ STATUS
        /// </summary>
        public string READSTATUS { get; set; }
      
        /// <summary>
        /// INBOXID(Transaction Primary Key)
        /// </summary>
        public int INBOXID { get; set; }
        /// <summary>
        /// COMMUNICATION XML
        /// </summary>
        public XElement COMMUNICATIONXML { get; set; }
        /// <summary>
        /// Ovveride the Primary key of the entity
        /// </summary>

        public override int Key
        {
            get { return EMAILID; }
            set { EMAILID = value; }
        }
        public List<M_5T_MAIL_COMPONENT> M_5T_MAIL_COMPONENT { get; set; }

        public int DEPTID { get; set; }
        public string ACTION { get; set; }
        public int CreatedBy { get; set; }
        public int FROMDEPT { get; set; }
        public int TODEPT { get; set; }
        public int fromreadstatus { get; set; }
        public int toreadstatus { get; set; }
        public string COMPONENTID { get; set; }
        public string ACTIONID { get; set; }
        public string filsize { get; set; }
        public string Status { get; set; }
        public string Size { get; set; }
        public string REPLAYID { get; set; }
        public string REPLAYMESSAGE { get; set; }
    }
    public class M_5T_MAIL_COMPONENT
    {
        /// <summary>
        /// MAILID( Primary Key)
        /// </summary>
        public int MAILID { get; set; }
        /// <summary>
        /// EMAILID(Mail Component Primary Key)
        /// </summary>
        public int EMAILID { get; set; }
        /// <summary>
        /// DEPTID(Department Id)
        /// </summary>
        public int COMPONENTID { get; set; }
    }
    public class COMMUNICATIONVIEW
    {
        public string deptname { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGE { get; set; }
        public string MAILDATE { get; set; }
        public int EMAILID { get; set; }
        public string Status { get; set; }

        public string COMPONENT { get; set; }
        public int MTOTAL { get; set; }
        public int TTOTAL { get; set; }
        public int ACTIONDEPARTMENT { get; set; }
        public string FILESIZE { get; set; }
        public string RID { get; set; }
        public string RMESSAGE { get; set; }


    }
    public class COMMUNICATIONINBOX
    {
        public List<CommunicationWrite> ViewInbox { get; set; }
        public List<COMMUNICATIONVIEW> TMAILINBOX { get; set; }      
    }
}

