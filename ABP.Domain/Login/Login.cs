using System;
using System.Collections.Generic;
using System.Text;

namespace ABP.Domain.Login
{
    public class Login : BaseEntity<int>
    {

        /// <summary>
        /// INTUSERID
        /// </summary>
        public int INTUSERID { get; set; }

        public int LOGID { get; set; }
        /// <summary>
        /// Ovveride the Primary key of the entity
        /// </summary>
        public override int Key
        {
            get { return INTUSERID; }
            set { INTUSERID = value; }
        }


        /// <summary>
        /// INTLEVELDETAILID
        /// </summary>
        public int INTLEVELDETAILID { get; set; }



        /// <summary>
        /// INTROLEID
        /// </summary>
        public int ROLEID { get; set; }


        /// <summary>
        /// VCHUSERNAME
        /// </summary>
        public string VCHUSERNAME { get; set; }

        public string encryptedtxtuname { get; set; }

        public string encryptedtxtpwd { get; set; }


        /// <summary>
        /// VCHFULLNAME
        /// </summary>
        public string vchfullname { get; set; }

        

        /// <summary>
        /// nvchdesigname
        /// </summary>

        public string nvchdesigname { get; set; }
        /// <summary>
        /// nvchaliasname
        /// </summary>
        public string nvchaliasname { get; set; }

        /// <summary>
        /// nvchlevelname
        /// </summary>
        public string nvchlevelname { get; set; }
        public int intdesigid { get; set; }
        public string vchpassword { get; set; }
        public string Message { get; set; }
        public string MOBILENO { get; set; }
        public string vchspcid { get; set; }
        public int INTLEVELID { get; set; }
        public int HRMSSTATUS { get; set; }
        public string HRMSUSERNAME { get; set; }
        public int DISTRICTID { get; set; }
        public int BLOCKID { get; set; }
        public string dist { get; set; }
        public int INTPARENTID { get; set; } 
        public string LoginTime { get; set; }
        public string LogoutTime { get; set; }
        public string DistrictName { get; set; }    
        public string BlockName { get; set; }

        public int Freqid { get; set; }

        public string FREQUENCYVALUE { get; set; }

        public string Month { get; set; }
        public string Year { get; set; }

        public float blockperfomance { get; set; }

        public float DISTPERFOMANCE { get; set; }
        public int CODE { get; set; }

        public string hdnUserName { get; set; }
    }
    public class LoginClass
    {
        public string username { get; set; }
        public string userpassword { get; set; }
    }
    public class LoginResponse
    {
        public int accountnonexpired { get; set; }
        public string linkid { get; set; }
        public int accountnonlocked { get; set; }
        public int enable { get; set; }
        public DateTime lastlogin { get; set; }
        public string usertype { get; set; }
        public int noofloginfailed { get; set; }
        public string securitykey { get; set; }
        public string message { get; set; }
        public int credentialsnonexpired { get; set; }
        public int userid { get; set; }
        public DateTime lastloginfailed { get; set; }
        public string usercredentials { get; set; }
    }
    public class CallbackResponse
    {
        public string linkid { get; set; }
        public string securitykey { get; set; }
        public int responseCode { get; set; }
        public string responseMsg { get; set; }
        public string responseData { get; set; }

        
    }
    //public class Responsedata
    //{
    //    public string curspc { get; set; }
    //    public string mobileNumber { get; set; }
    //    public string curspn { get; set; }
    //    public string name { get; set; }
    //    public string emailId { get; set; }
    //    public string designation { get; set; }
    //    public string employeeAdditionalCharge { get; set; }
    //}

}
