using ABP.Domain.Frequency;
using ABP.Domain.Panel;
using ABP.Domain.Sector;
using ABP.Persistence;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Repository.Sector;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ABP.Web.Controllers
{
    public class SectorController : Controller
    {
        private readonly ILogger<SectorRepository> Logger;
        private readonly ISectorRepository _sectorReportingRepository;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IPanelRepository _panelRepository;
        public IConfiguration Configuration { get; }
        public SectorController(ILogger<SectorRepository> logger, IHostingEnvironment hostingEnvironment, ISectorRepository sectorReportingRepository, IConfiguration configuration, IPanelRepository panelRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _sectorReportingRepository = sectorReportingRepository;
            _panelRepository = panelRepository;
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("SectorController initialized");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddSector()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.Department = _panelRepository.ViewLevels().Result;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddSector(sector sector)
        {
            try
            {
                Panel panel = new Panel();
                string result = string.Empty;
                ViewBag.Department = _panelRepository.ViewLevels().Result;
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
                        //sector = new sector();                                               
                        try
                        {
                            string flodername = "SectorDocuments";
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
                                fullname = "Sector" + timestamp + "" + filename;
                                string k = CommonFunction.IsImageValid1(file[0]);
                                if (k == "false")
                                {
                                    return Json("This is not a valid Image file.");
                                }
                                using (var stream = new FileStream(Path.Combine(ProcDocPath, fullname), FileMode.Create))
                                {
                                    file[0].CopyTo(stream);
                                }
                            }
                            else
                            {
                                fullname = sector.CSSCLASS;
                            }
                            panel.DISPLAYNAME = HttpContext.Request.Form["SECTORNAME"].ToString();
                            //panel.DEPTID = Convert.ToInt32(HttpContext.Request.Form["DEPTID"]); 
                            panel.CREATEDBY = Convert.ToInt32(value);
                            panel.PANELID =Convert.ToInt32(HttpContext.Request.Form["SECTORID"].ToString());
                            panel.IMAGE = fullname;
                            if(panel.PANELID==0)
                            {
                                string SeqVal = _panelRepository.GetSeqValue("PANELNAME_SEQ");
                                panel.PANELNAME = "PANEL_" + SeqVal;
                            }    
                            string retMsg = _panelRepository.InsertPanel(panel);
                            if(retMsg=="1")
                            {
                                string ret = _panelRepository.CreateTable("T_ABP_"+panel.PANELNAME);
                                if(ret=="1")
                                {
                                    return Json("Record Saved Successfully");
                                }
                                else
                                {
                                    return Json("Failled To Create Table");
                                }
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
                            Logger.LogError(ex, ex.Message);
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
                Logger.LogError(ex, ex.Message);
                throw ex;
            }           
        }

        public IActionResult ViewSector()
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    List<Panel> panels = ViewBag.Result = _panelRepository.ViewPanel().Result.ToList();
                    sector sector = new sector();
                    ViewBag.Department = _panelRepository.ViewLevels().Result;
                    List<sector> sectors = (from e in panels
                                            select new sector
                                            {
                                                SECTORID = e.PANELID,
                                                SECTORNAME = e.DISPLAYNAME,
                                                CSSCLASS = e.IMAGE
                                                //Deptid=e.DEPTID,
                                                //DEPTNAME=e.DEPTNAME
                                            }).ToList();
                    
                    ViewBag.Result = sectors;
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        [HttpPost]
        public IActionResult DeleteSector(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    Panel model = new Panel();
                    model.PANELID = id;                   
                    string Result = _panelRepository.DeletePanel(model);
                    return Json(Result);
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch(Exception Ex)
            {
                throw Ex;
            }

        }
        [HttpGet]
        public IActionResult SectorGateById(string SectorId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                Panel panel =ViewBag.Panels= _panelRepository.PanelGateById(Convert.ToInt32(SectorId)).Result.FirstOrDefault();
                ViewBag.Department = _panelRepository.ViewLevels().Result;
                sector sector = new sector();
                List<sector> sectors = new List<sector>();
                if(panel!=null)
                {
                    sector.SECTORNAME = panel.DISPLAYNAME;
                    //sector.Deptid = panel.DEPTID;
                    //sector.DEPTNAME = panel.DEPTNAME;
                    sector.SECTORID = panel.PANELID;
                    sector.CSSCLASS = panel.IMAGE;
                    sectors.Add(sector);
                }
                return Ok(JsonConvert.SerializeObject(sectors));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}
