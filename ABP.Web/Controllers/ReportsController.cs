using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ABP.Domain.Login;
using ABP.Domain.QueryBuilder;
using ABP.Repository.Contract.Contract.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ClientsideEncryption;
using Oracle.ManagedDataAccess.Client;

namespace ABP.Web.Controllers
{
    public class ReportsController : Controller
    {
        public bool IsDML { get; set; }
        private readonly ILoginRepository _loginRepository;
        private readonly ILogger<ReportsController> Logger;
        public IConfiguration Configuration { get; }
        // public new OracleTransaction BeginTransaction();

        public ReportsController(ILoginRepository loginRepository, IConfiguration configuration, ILogger<ReportsController> logger)
        {
            _loginRepository = loginRepository;
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("QueryBuilderController initialized");
        }
        public IActionResult CheckDMLCode(string dcode)
        {
            try
            {
                string dmlcode = Configuration.GetValue<string>("QueryBuilder:DMLACode");
                if (dmlcode == dcode)
                {
                    return Ok(new
                    {
                        status = 1,
                        dcode = dmlcode
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = 0,
                        dcode = ""
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IActionResult AnalyticReport(string code)
        {
            try
            {
                ViewBag.data = code;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult AnalyticReport(QueryBuilderModel objQuerybuilder)
        {
            try
            {
                string strQuery = objQuerybuilder.QueryText;
                if (strQuery == "" || strQuery == null)
                {
                    ViewBag.showmessage = "Query Textbox can not be left blank!!!";

                    return View();
                }
                else
                {
                    if (!IsValidQuery(strQuery, objQuerybuilder.DMLCode))
                    {
                        ViewBag.data = objQuerybuilder.DMLCode;
                        ViewBag.showmessage = "You are not authorized to perform any DML operation. A notification has been sent to administrator regarding such accessibility. Please contact your administrator for authorization.";

                        return View();
                    }
                    else
                    {
                        if (IsDML == true)
                        {
                            int affectedRow = RunOracleTransactionExecuteNonQuery(strQuery);// ExecuteNonQuery(strQuery);
                            objQuerybuilder.QueryText = "";
                            if (affectedRow > 0)
                            {
                                ViewBag.showmessage = $"Command executed successfully...{ affectedRow} Rows affected";
                                ViewBag.data = objQuerybuilder.DMLCode;
                                return View();

                            }
                            else if (affectedRow == -1)
                            {
                                ViewBag.showmessage = $"Command executed successfully...";
                                ViewBag.data = objQuerybuilder.DMLCode;
                                return View();

                            }
                            else
                            {
                                //ViewBag.showmessage = TempData["Message"].ToString();
                                ViewBag.showmessage = "0";
                                ViewBag.data = objQuerybuilder.DMLCode;
                                return View();
                            }


                        }
                        else
                        {
                            ViewBag.dmlcode = Configuration.GetValue<string>("QueryBuilder:DMLACode");
                            DataTable dtData = GetData(strQuery);
                            return PartialView("_GetMyData", dtData);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int RunOracleTransactionExecuteNonQuery(string query)
        {
            try
            {
                string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(this.Configuration, "ABPConnectionString");
                using (OracleConnection connection = new OracleConnection(conString))
                {
                    connection.Open();
                    int affectedRow = 0;
                    OracleCommand command = connection.CreateCommand();
                    OracleTransaction transaction;

                    // Start a local transaction
                    transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    // Assign transaction object for a pending local transaction
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = query;
                        affectedRow = command.ExecuteNonQuery();
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["Message"] = ex.Message;
                        TempData.Keep();

                        Logger.LogError(ex, "Error while executing the query", query);

                    }
                    return affectedRow;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable GetData(string query)
        {
            string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(this.Configuration, "ABPConnectionString");
            DataTable dt = new DataTable();
            string oradb = conString;
            //string oradb = "Data Source = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = Ora3.corp.csmpl.com)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = ORA3.corp.csmpl.com))); User Id = CDash; Password = Cm5TDaM#348";// "Data Source=ORCL;User Id=hr;Password=hr;";string oradb = "Data Source = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = Ora3.corp.csmpl.com)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = ORA3.corp.csmpl.com))); User Id = CDash; Password = Cm5TDaM#348";// "Data Source=ORCL;User Id=hr;Password=hr;";
            using (OracleConnection conn = new OracleConnection(oradb))
                try
                {

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }


                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error while executing the query", query);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            return dt;
        }


        private bool IsValidQuery(string query, string authority)
        {
            try
            {
                if((query.ToLower().Contains("update ") && query.ToLower().Contains("where ")) ||
                    query.ToLower().Contains("insert ") ||
                    query.ToLower().Contains("create ") ||
                    query.ToLower().Contains("alter ") ||
                    (query.ToLower().Contains("delete ") && query.ToLower().Contains("where ")) ||
                    //query.ToLower().Contains("truncate ") ||
                    //query.ToLower().Contains("drop ") ||
                    query.ToLower().Contains("exec "))
                {
                    IsDML = true;
                    //var dmlcode = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("QueryBuilder")["DMLACode"];
                    //if (authority == dmlcode)
                    //{
                    //    return true;
                    //}
                    //else
                    //    return authority == DateTime.Today.ToString("ddMMyyyy") + "5T";
                    return true;
                }
                else if (query.ToLower().Contains("select "))
                {
                    IsDML = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
