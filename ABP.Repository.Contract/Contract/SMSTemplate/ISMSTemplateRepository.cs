using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Repository.Contract.Contract.SMSTemplate
{
    public interface ISMSTemplateRepository : IDisposable, IRepository
    {
        Task<IEnumerable<ABP.Domain.SMSTemplate.SMSTemplate>> GetTimePeriod();
        //List<ABP.Domain.SMSTemplate.SMSTemplate> GetTemplatesByDay(ABP.Domain.SMSTemplate.SMSTemplate sMSday);
        List<Domain.SMSTemplate.SMSTemplate> GetTemplatesByDay(Domain.SMSTemplate.SMSTemplate sMS);
        string InsertSMSTemplate(ABP.Domain.SMS.SMSData sms);
        string UpdateSMSTemplate(ABP.Domain.SMS.SMSData sms);
        string InsertSMSTemplateDetail(ABP.Domain.SMS.SMSData sms);

        string InsertSendSMSDetail(int DISTRICTID, int BLOCKID,string Mobno,string retMsg);


        Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewSMSTemplates();

        Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewTemplate();

        Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewAllSmsTemplate(DateTime smsdata);
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetEventDetails();
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetTemplateDetails();
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetUserDetailsForSMS(ABP.Domain.SMS.SMSData sms);
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> GetAllUserDetailsForSMS();
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> PostSendSMS();
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> PostSendSMSCol(int distid);
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> PostSMScol();
        // int InsertJobRecord(int distid,int freqid,int status,string jobtype);
        string InsertJobRecord(int distid, int freqid, int status, string jobtype);
        string UpdateJobRecord(int status,int Jobid);
        string InsertIndicatorJobDetails(ABP.Domain.SMS.SMSData sms);
        Task<IEnumerable<ABP.Domain.SMS.SMSData>> ViewIndicatorJobDetails();
        string DeleteIndicatorJobDetails(int ID);
    }
}
