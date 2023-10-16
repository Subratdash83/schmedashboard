using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ABP.Domain;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
/// <summary>
/// Summary description for CommonFunction
/// </summary>


namespace ABP.Persistence
{
    public class CommonFunction
    {
        #region Member Variables
        static string strDecodeMatch = "X+HIcwPJE/4=";
        //static readonly Byte[] hashedBytes;

        public static UTF8Encoding Encoder { get; set; } = new UTF8Encoding();
        public static MD5CryptoServiceProvider Md5Hasher { get; set; } = new MD5CryptoServiceProvider();
        #endregion
        public static string GetDecodedData(string strDataToDecode)
        {
            string[] decodeArray = strDataToDecode.Split(' ');
            strDataToDecode = null;
            for (int i = 0; i < decodeArray.Length; i++)
            {
                if (decodeArray[i].ToString() == strDecodeMatch)
                {
                    strDataToDecode += "&";
                    strDataToDecode += " ";
                }
                else
                {
                    strDataToDecode += decodeArray[i].ToString();
                    strDataToDecode += " ";
                }
            }
            return strDataToDecode;
        }
        //*******************Summery****************************
        //Function Name             : EncryptData(),DecryptData()
        //Purpose                   : Get the GlobalLinkId by logined user
        //InPut Parameters Name     :    toEncrypt,  cipherString
        //InPut Parameters DataType : string
        //OutPut Parameters Name    : None
        //OutPut Parameters DataType: None
        //Retun  Value              : encodeData,decodedData
        //Retun Datatype            : string
        //Created By                : Dilip Kumar Tripathy
        //Created Date              : 09-JAN-2012
        //*****************************************************
        #region Encrypt & Decrypt method

        public static string EncryptData(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            string key = "!@#$%^&*()_+~`";
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            AesCryptoServiceProvider oTripleDESCryptoServiceProvider = new AesCryptoServiceProvider();
            oTripleDESCryptoServiceProvider.Key = keyArray;
            oTripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
            oTripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = oTripleDESCryptoServiceProvider.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            oTripleDESCryptoServiceProvider.Clear();
            string encodeData = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            return encodeData;
        }
        public static string DecryptData(string cipherString)
        {
            if (cipherString != "")
            {
                cipherString = cipherString.Replace(" ", "+");
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);
                string key = "!@#$%^&*()_+~`";
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
                AesCryptoServiceProvider tdes = new AesCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                string decodedData = UTF8Encoding.UTF8.GetString(resultArray);
                return decodedData;
            }
            else
            {
                return "";
            }

        }

        public static string DecryptBase64String(string encryptedstring)
        {
            byte[] bytes = Convert.FromBase64String(encryptedstring);
            string decodedString = Encoding.Unicode.GetString(bytes);
            return decodedString;
        }
        public static string Encrypt(string clearText)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                    throw ex;
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion
        public static string IsFileValid(IFormFile FileUpload1)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(FileUpload1.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)FileUpload1.Length);

            string[] allowedImageTyps = { "application/pdf" };
            string[] allowedExtension = { ".pdf" };
            StringCollection imageTypes = new StringCollection();
            StringCollection imageExtension = new StringCollection();
            imageTypes.AddRange(allowedImageTyps);
            imageExtension.AddRange(allowedExtension);
            string strFiletype = MimeType.GetMimeType(CoverImageBytes, FileUpload1.FileName);
            string fileExt = System.IO.Path.GetExtension(FileUpload1.FileName);//
            int count = FileUpload1.FileName.Count(f => f == '.');

            //string filename = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.FileName);

            if (imageTypes.Contains(strFiletype) && imageExtension.Contains(fileExt) && count == 1)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }        

        public static string IsImageValid(IFormFile FileUpload1)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(FileUpload1.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)FileUpload1.Length);
            string[] allowedImageTyps = { "image/jpg", "image/jpeg", "image/png", "image/gif" };
            string[] allowedExtension = { ".jpg", ".jpeg", ".png", ".gif" };
            StringCollection imageTypes = new StringCollection();
            StringCollection imageExtension = new StringCollection();
            imageTypes.AddRange(allowedImageTyps);
            imageExtension.AddRange(allowedExtension);
            string strFiletype = MimeType.GetMimeType(CoverImageBytes, FileUpload1.FileName);
            string fileExt = System.IO.Path.GetExtension(FileUpload1.FileName);//
            int count = FileUpload1.FileName.Count(f => f == '.');

            if (imageTypes.Contains(strFiletype) && imageExtension.Contains(fileExt) && count == 1)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }
        public static string IsImageValid1(IFormFile FileUpload1)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(FileUpload1.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)FileUpload1.Length);
            string[] allowedImageTyps = { "image/png" };
            string[] allowedExtension = { ".png" };
            StringCollection imageTypes = new StringCollection();
            StringCollection imageExtension = new StringCollection();
            imageTypes.AddRange(allowedImageTyps);
            imageExtension.AddRange(allowedExtension);
            string strFiletype = MimeType.GetMimeType(CoverImageBytes, FileUpload1.FileName);
            string fileExt = System.IO.Path.GetExtension(FileUpload1.FileName);//
            int count = FileUpload1.FileName.Count(f => f == '.');

            if (imageTypes.Contains(strFiletype) && imageExtension.Contains(fileExt) && count == 1)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        public static string chkSpecialchar(string a)
        {
            string chkspecialchar = @"[^<>='][a-zA-Z\-$!@#%&()_.,\/\ \w]+$";
            Regex re = new Regex(chkspecialchar);
            if(!re.IsMatch(a))
            {
                return "not ok";
            }
            else
            {
                return "ok";
            }
        }

        public static string checkDate(string a)
        {
            string chkDt = @"^\d{0,9}\-[a-zA-Z]{3}\-\d{4}$";
            Regex Dt = new Regex(chkDt);
            if (!Dt.IsMatch(a))
            {
                return "not ok";
            }
            else
            {
                return "ok";
            }
        }

        public static string chkDept(string a)
        {
            string onlynum = @"^([1-9]?[0-9]|100)$";
            Regex number = new Regex(onlynum);
            if (!number.IsMatch(a))
            {
                return "not ok";
            }
            else
            {
                return "ok";
            }
        }

        public static string chkNumeric(string a)
        {
            string chknumeric = @"^(0|[1-9][0-9]*)$";           
            Regex numeric = new Regex(chknumeric);
            if (!numeric.IsMatch(a))
            {
                return "ok";
            }
            else
            {
                return "not ok";
            }
        }

        public static List<string> iCals(List<Calender> iCals)
        {
            List<string> calendars = new List<string>();
            foreach (Calender iCal in iCals)
            {
                StringBuilder sb = new StringBuilder();
                //Calendar
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("PRODID:-//Compnay Inc//Product Application//EN");
                sb.AppendLine("VERSION:2.0");
                sb.AppendLine("METHOD:REQUEST");
                //Event
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + toUniversalTime(iCal.EventStartDateTime));
                sb.AppendLine("DTEND:" + toUniversalTime(iCal.EventEndDateTime));
                sb.AppendLine("DTSTAMP:" + toUniversalTime(iCal.EventTimeStamp));
                sb.AppendLine("ORGANIZER;CN=John Doe:mailto:"+iCal.ToMail);
                sb.AppendLine("UID:" + iCal.UID);               
                sb.AppendLine("CREATED:" + toUniversalTime(iCal.EventCreatedDateTime));
                sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + iCal.EventDescription);
                sb.AppendLine("LAST-MODIFIED:" + toUniversalTime(iCal.EventLastModifiedTimeStamp));
                sb.AppendLine("LOCATION:" + iCal.EventLocation);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + iCal.EventSummary);
                sb.AppendLine("TRANSP:OPAQUE");
                //Alarm
                sb.AppendLine("BEGIN:VALARM");
                sb.AppendLine("TRIGGER:" + String.Format("-PT{0}M", iCal.AlarmTrigger));
                sb.AppendLine("REPEAT:" + iCal.AlarmRepeat);
                sb.AppendLine("DURATION:" + String.Format("PT{0}M", iCal.AlarmDuration));
                sb.AppendLine("ACTION:DISPLAY");
                sb.AppendLine("DESCRIPTION:" + iCal.AlarmDescription);
                sb.AppendLine("END:VALARM");
                sb.AppendLine("END:VEVENT");
                sb.AppendLine("END:VCALENDAR");
                calendars.Add(sb.ToString());
            }
            return calendars;
        }
        public static string toUniversalTime(DateTime dt)
        {
            string DateFormat = "yyyyMMddTHHmmssZ";
            return dt.ToUniversalTime().ToString(DateFormat);
        }

        public static bool SendEmail(string strSubject, string strBody,  string toEmail, bool enbleSSl,string SmtpHost,string FromEmail,string Port,string Password, Attachment attach)
        {           
            bool Res = false;
            try
            {
                MailMessage mail = new MailMessage();

                SmtpClient SmtpServer = new SmtpClient(SmtpHost);

                mail.From = new MailAddress(FromEmail);

                //for (int i = 0; i < toEmail.Length; i++)
                //{
                    mail.To.Add(toEmail);
                //}
                mail.Subject = strSubject;

                mail.Body = strBody;

                if (attach != null)
                {
                    mail.Attachments.Add(attach);
                }

                SmtpServer.Port = Convert.ToInt32(Port);

                SmtpServer.Credentials = new System.Net.NetworkCredential(FromEmail, Password);

                SmtpServer.EnableSsl =enbleSSl;

                mail.IsBodyHtml = true;

                //Warning: do not use this in production code!

                //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)

                //{ 
                //    return true; 
                //};
                //mail.Attachments.Add(attach);

                SmtpServer.Send(mail);

                Res = true;

            }
            catch(Exception ex)
            {
                Res = false;
            }
            return Res;
        }
    }
}

