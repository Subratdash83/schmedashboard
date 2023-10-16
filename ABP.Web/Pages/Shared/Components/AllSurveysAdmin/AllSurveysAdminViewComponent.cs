using ABP.Repository.Contract.Common;
using ABP.Repository.Contract.Contract.AdminConsole;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Pages.Shared.Components.AllSurveysAdmin
{
    public class AllSurveysAdminViewComponent :ViewComponent
    {
        private IAdminConsoleRepository _IAdminConsoleRepository { get; }
        private readonly IConfiguration _config;
        private readonly ICommonRepository _ICommonRepository;

        public AllSurveysAdminViewComponent(IAdminConsoleRepository IAdminConsoleRepository, IConfiguration config, ICommonRepository ICommonRepository)
        {
            _IAdminConsoleRepository = IAdminConsoleRepository;
            _ICommonRepository = ICommonRepository;
            _config = config;
        }
        public async Task<IViewComponentResult> InvokeAsync(string ratingControlType)
        {
            //var userid = this.UserClaimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals("userid", StringComparison.OrdinalIgnoreCase))?.Value;
            var userid = HttpContext.Session.GetInt32("_UserId");
            var desgid = HttpContext.Session.GetInt32("_DesignId");
            var PId = _config.GetValue<int>("MySettings:ProjectId");
            var results = _IAdminConsoleRepository.GetLinkAccessByUserId(Convert.ToInt32(userid), PId, Convert.ToInt32(desgid));
            var Data = results.OrderBy(x => x.intglinkid != 21).ToList();
            if (HttpContext.Request.Query["Glink"].ToString() != "")
            {
                //Data[0].Aglinkid = Convert.ToInt32(HttpContext.Request.Query["Glink"]);
                Data.ForEach(c => c.Aglinkid = Convert.ToInt32(HttpContext.Request.Query["Glink"]));
                //HttpContext.Session.SetInt32("Glink", Convert.ToInt32(HttpContext.Request.Query["Glink"]));
                TempData["Glink"] = HttpContext.Request.Query["Glink"].ToString();
                TempData.Keep();
            }
            if (HttpContext.Request.Query["Plink"].ToString() != "")
            {
                //Data[0].Aplinkid = Convert.ToInt32(HttpContext.Request.Query["Plink"]);
                Data.ForEach(c => c.Aplinkid = Convert.ToInt32(HttpContext.Request.Query["Plink"]));
                //HttpContext.Session.SetInt32("Plink", Convert.ToInt32(HttpContext.Request.Query["Plink"]));
                TempData["Plink"] = HttpContext.Request.Query["Plink"].ToString();
                TempData.Keep();
            }
            return View("default", results);
            //var surveys = await _iDFormRepository.GetSurveys();
            //return View("default", surveys.OrderBy(x => x.ID));
        }

        //public async Task<IViewComponentResult> InvokeAsync(string ratingControlType)  //-----(USER AUTHENTICATION)----
        //{
        //    try
        //    {
        //        var PLink = 0;
        //        //var userid = this.UserClaimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals("userid", StringComparison.OrdinalIgnoreCase))?.Value;
        //        string path = HttpContext.Request.Path;
        //        var userid = Convert.ToInt32(HttpContext.Session.GetInt32("_UserId"));
        //        var desgid = Convert.ToInt32(HttpContext.Session.GetInt32("_DesignId"));
        //        var plink = HttpContext.Request.Query["Plink"].ToString();
        //        if (plink != "")
        //        {
        //            PLink = Convert.ToInt32(plink);
        //        }
        //        var PId = _config.GetValue<int>("MySettings:ProjectId");
        //        var results = _IAdminConsoleRepository.GetLinkAccessByUserId(Convert.ToInt32(userid), PId, Convert.ToInt32(desgid));
        //        var Data = results.ToList();

        //        var userDesigcheck = _ICommonRepository.UserAccessCheck(PLink, desgid, userid, 0).Result;
        //        var Desigdata = userDesigcheck.ToList()[0];
        //        var userUseridcheck = _ICommonRepository.UserAccessCheck(PLink, desgid, userid, 1);
        //        var Userdata = userUseridcheck.Result.ToList()[0];
        //        bool isinitial = false;
        //        if (HttpContext.Session.GetInt32("IntialLogin") == 1)
        //        {
        //            isinitial = true;
        //            HttpContext.Session.SetInt32("IntialLogin", 0);
        //        }
        //        try
        //        {
        //            if (isinitial || path == ("/Home/BlockDashboard") || path == ("/Home/DistDashboard") || path == ("/Home/Dashboard") || path == ("/DistrictData/Pending") || path == ("/DistrictData/Pending") || path ==("/DistrictData/Approved"))
        //            {
        //                if (HttpContext.Request.Query["Glink"].ToString() != "")
        //                {
        //                    //Data[0].Aglinkid = Convert.ToInt32(HttpContext.Request.Query["Glink"]);
        //                    Data.ForEach(c => c.Aglinkid = Convert.ToInt32(HttpContext.Request.Query["Glink"]));
        //                    //HttpContext.Session.SetInt32("Glink", Convert.ToInt32(HttpContext.Request.Query["Glink"]));
        //                    TempData["Glink"] = HttpContext.Request.Query["Glink"].ToString();
        //                    TempData.Keep();
        //                }
        //                if (HttpContext.Request.Query["Plink"].ToString() != "")
        //                {
        //                    //Data[0].Aplinkid = Convert.ToInt32(HttpContext.Request.Query["Plink"]);
        //                    Data.ForEach(c => c.Aplinkid = Convert.ToInt32(HttpContext.Request.Query["Plink"]));
        //                    //HttpContext.Session.SetInt32("Plink", Convert.ToInt32(HttpContext.Request.Query["Plink"]));
        //                    TempData["Plink"] = HttpContext.Request.Query["Plink"].ToString();
        //                    TempData.Keep();
        //                }
        //                ViewBag.MSG = "IN";
        //            }
        //            else
        //            {
        //                if (Desigdata >= 1 || Userdata >= 1)
        //                {
        //                    if (HttpContext.Request.Query["Glink"].ToString() != "")
        //                    {
        //                        //Data[0].Aglinkid = Convert.ToInt32(HttpContext.Request.Query["Glink"]);
        //                        Data.ForEach(c => c.Aglinkid = Convert.ToInt32(HttpContext.Request.Query["Glink"]));
        //                        //HttpContext.Session.SetInt32("Glink", Convert.ToInt32(HttpContext.Request.Query["Glink"]));
        //                        TempData["Glink"] = HttpContext.Request.Query["Glink"].ToString();
        //                        TempData.Keep();
        //                    }
        //                    if (HttpContext.Request.Query["Plink"].ToString() != "")
        //                    {
        //                        //Data[0].Aplinkid = Convert.ToInt32(HttpContext.Request.Query["Plink"]);
        //                        Data.ForEach(c => c.Aplinkid = Convert.ToInt32(HttpContext.Request.Query["Plink"]));
        //                        //HttpContext.Session.SetInt32("Plink", Convert.ToInt32(HttpContext.Request.Query["Plink"]));
        //                        TempData["Plink"] = HttpContext.Request.Query["Plink"].ToString();
        //                        TempData.Keep();
        //                    }
        //                    ViewBag.MSG = "IN";
        //                }
        //                else
        //                {
        //                    ViewBag.MSG = "OUT";
        //                }
        //            }
        //        }
        //        catch
        //        {


        //        }
        //        return View("default", results);
        //        //return View("default", results);
        //        //var surveys = await _iDFormRepository.GetSurveys();
        //        //return View("default", surveys.OrderBy(x => x.ID));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
    }
}
