using ABP.Domain.DataPointMapping;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Panel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Controllers
{
    public class DataPointMappingController : Controller
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IControlPanelRepository _controlPanelRepository;
        public IConfiguration Configuration { get; }
        public DataPointMappingController(IHostingEnvironment hostingEnvironment, IIndicatorRepository indicatorRepository, IConfiguration configuration, IPanelRepository panelRepository, IControlPanelRepository controlPanelRepository)
        {
            Configuration = configuration;
            _panelRepository = panelRepository;
            _indicatorRepository = indicatorRepository;
            _controlPanelRepository = controlPanelRepository;
        }
        public IActionResult DataPointMapping()
        {
            ViewBag.Sector = _panelRepository.ViewPanel().Result;//Binding Initials
            ViewBag.Indicator = _indicatorRepository.ViewIndicator(0,0,0).Result;//Binding Initials
            //ViewBag.DataPoint = _controlPanelRepository.ViewControlPanel().Result;//Binding Initials
            return View();
        }
        //[HttpPost]
        //public async Task<JsonResult> DataPointMapping(DataPointMapping dpMapping)
        //{
        //    try
        //    {
        //        string result = string.Empty;
        //        var value = HttpContext.Session.GetInt32("_UserId");
        //        var errors = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
        //        if (!ModelState.IsValid)
        //        {
        //            return Json(errors.ErrorMessage);

        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(value.ToString()))
        //            {
        //                dpMapping = new DataPointMapping();
        //                try
        //                {
        //                    dpMapping.PANELID = Convert.ToInt32(HttpContext.Request.Form["PANELID"]);
        //                    dpMapping.INDICATORID = Convert.ToInt32(HttpContext.Request.Form["SECTORID"]);
        //                    dpMapping.DATAPOINTS = HttpContext.Request.Form["INDICATORNAME"].ToString();
        //                    string retMsg = _indicatorRepository.InsertIndicator(indicator);
        //                    if (retMsg != "0")
        //                    {
        //                        Department department = new Department();
        //                        department.ID = Convert.ToInt32(HttpContext.Request.Form["ID"]);
        //                        department.SECTORID = Convert.ToInt32(HttpContext.Request.Form["SECTORID"]);
        //                        department.INDICATORID = Convert.ToInt32(retMsg);
        //                        department.DEPTID = Convert.ToInt32(HttpContext.Request.Form["DEPTID"]);
        //                        department.DESCRIPTION = HttpContext.Request.Form["DESCRIPTION"].ToString();
        //                        department.CREATEDBY = Convert.ToInt32(value);
        //                        string retMsg2 = _dpartmentRepository.AddDepartment(department);

        //                        if (retMsg2 == "1")
        //                        {
        //                            return Json("Record Saved Successfully");
        //                        }
        //                        else if (retMsg2 == "2")
        //                        {
        //                            return Json("Record Updated Successfully");
        //                        }
        //                        else if (retMsg2 == "3")
        //                        {
        //                            return Json("Record Deleted Successfully");
        //                        }
        //                        else
        //                        {
        //                            return Json("Record Already Exist");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return Json("Some Error Occured in Department Mapping");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Logger.LogError(ex, ex.Message);
        //                    result = ex.Message;
        //                    return Json(ex.Message);
        //                }
        //            }
        //            else
        //            {
        //                RedirectToAction("Logout", "Home");
        //                return Json(null);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, ex.Message);
        //        throw ex;
        //    }
        //}
    }
}
