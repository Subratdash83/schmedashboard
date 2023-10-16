using ABP.Domain.Panel;
using ABP.Repository.Contract.Contract.Panel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ABP.Web.Controllers
{
    public class PanelController : Controller
    {
        private readonly IPanelRepository _PanelReportingRepository;
        private IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public PanelController(IHostingEnvironment hostingEnvironment, IPanelRepository PanelReportingRepository, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _PanelReportingRepository = PanelReportingRepository;
            Configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddPanel()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddPanel(Panel Panel)
        {
            try
            {
                string result = string.Empty;
                var value = HttpContext.Session.GetInt32("_UserId");
                var errors = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                if (!ModelState.IsValid)
                {
                    return Json(errors.ErrorMessage);

                }
                else
                {
                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        //Panel = new Panel();                                               
                        try
                        {
                            string flodername = "PanelDocuments";
                            string webrootpath = _hostingEnvironment.WebRootPath;
                            string ProcDocPath = Path.Combine(webrootpath, flodername);
                            if (!Directory.Exists(ProcDocPath))
                                Directory.CreateDirectory(ProcDocPath);
                            var file = Request.Form.Files;
                            // file.CopyTo(file);


                            var fullname = "";
                            if (file.Count > 0)
                            {
                                var filename = Path.GetExtension(ContentDispositionHeaderValue.Parse(file[0].ContentDisposition).FileName.Trim('"'));
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                fullname = "Panel" + timestamp + "" + filename;
                                using (var stream = new FileStream(Path.Combine(ProcDocPath, fullname), FileMode.Create))
                                {
                                    file[0].CopyTo(stream);
                                }
                            }
                            else
                            {
                                fullname = Panel.IMAGE;
                            }

                            Panel.PANELNAME = HttpContext.Request.Form["PANELNAME"].ToString();
                            Panel.EFFECTIVADATE = HttpContext.Request.Form["EFFECTIVADATE"].ToString();
                            Panel.CREATEDBY = Convert.ToInt32(value);
                            Panel.PANELID = Convert.ToInt32(HttpContext.Request.Form["PANELID"].ToString());
                            Panel.IMAGE = fullname;
                            //HttpContext.Request.Form["CSSCLASS"].ToString();
                            string retMsg = _PanelReportingRepository.InsertPanel(Panel);
                            if (retMsg == "1")
                            {
                                return Json("Record Saved Successfully");
                            }
                            else if (retMsg == "2")
                            {
                                return Json("Record Updated Successfully");
                            }
                            else if (retMsg == "3")
                            {
                                return Json("Record Deleted Successfully");
                            }
                            else
                            {
                                return Json("Record Already Exist");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.StackTrace + "\n"+ex.Message);
                            result = ex.Message;
                            return Json(ex.Message);
                        }
                    }
                    else
                    {
                        RedirectToAction("Logout", "Home");
                        return Json(null);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace + "\n" + ex.Message);
                throw ex;
            }
        }

        public IActionResult ViewPanel()
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    ViewBag.Result = _PanelReportingRepository.ViewPanel();//getting all the Panels to bind in the grid
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public IActionResult DeletePanel(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    Panel model = new Panel();
                    model.PANELID = id;
                    model.CREATEDBY = int.Parse(value.ToString());
                    string Result = _PanelReportingRepository.DeletePanel(model);
                    return Json(Result);
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }
        [HttpGet]
        public IActionResult PanelGateById(string PanelId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Panels = _PanelReportingRepository.PanelGateById(Convert.ToInt32(PanelId)).Result;
                return Ok(JsonConvert.SerializeObject(Panels));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
