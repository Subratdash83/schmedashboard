using ABP.Repository;
using ABP.Repository.Contract.Factory;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ABP.Infrastructure
{
    public class SendSMSRepository: RepositoryBase, ISendSMSRepository
    {
        private readonly ILogger<SendSMSRepository> Logger;
        public SendSMSRepository(IConnectionFactory connectionFactory, ILogger<SendSMSRepository> logger) : base(connectionFactory)
        {
            Logger = logger;
            Logger.LogInformation("SendSMS initialized");
        }
        public async Task<string> Sendsms(string mobileno, string Content, string templateid)
        {
            string strusername = "opscsms2012-ODIGOV";
            string strPassword = "Odisha#2018#";
            string senderid = "ODIGOV";
            string SecureKey = "88f6ae42-6a35-46d1-a038-bee1fdad08d7";

            string templateID = templateid;
            //string _URL = "192.168.1";
            try
            {
                Stream dataStream;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequestDLT");
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 1;
                //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
                ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";
                request.Method = "POST";
                //System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                String encryptedPassword = encryptedPasswod(strPassword);
                String NewsecureKey = hashGenerator(strusername.Trim(), senderid.Trim(), Content.Trim(), SecureKey.Trim());
                String smsservicetype = "singlemsg"; //For single message.
                String query = "username=" + HttpUtility.UrlEncode(strusername.Trim()) +
                    "&password=" + HttpUtility.UrlEncode(encryptedPassword) +
                    "&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +
                    "&content=" + HttpUtility.UrlEncode(Content.Trim()) +
                    "&mobileno=" + HttpUtility.UrlEncode(mobileno) +
                    "&senderid=" + HttpUtility.UrlEncode(senderid.Trim()) +
                    "&key=" + HttpUtility.UrlEncode(NewsecureKey.Trim()) +
                    "&templateid=" + HttpUtility.UrlEncode(templateID.Trim());
                byte[] byteArray = Encoding.ASCII.GetBytes(query);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                String Status = ((HttpWebResponse)response).StatusDescription;
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                String responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                //return Status;
                return query;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
        }
        protected string encryptedPasswod(string password)
        {
            byte[] encPwd = Encoding.UTF8.GetBytes(password);
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA1");
            byte[] pp = sha1.ComputeHash(encPwd);
            // static string result = System.Text.Encoding.UTF8.GetString(pp);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in pp)
            {

                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();

        }

        protected string hashGenerator(string Username, string sender_id, string message, string secure_key)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(Username).Append(sender_id).Append(message).Append(secure_key);
            byte[] genkey = Encoding.UTF8.GetBytes(sb.ToString());
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA512");
            byte[] sec_key = sha1.ComputeHash(genkey);

            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < sec_key.Length; i++)
            {
                sb1.Append(sec_key[i].ToString("x2"));
            }
            return sb1.ToString();
        }
        
    }
}
