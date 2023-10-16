using ABP.Domain.DataPointExpression;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Panel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Controllers
{
    public class DatapointExpressionController : Controller

    {
        private readonly IPanelRepository _panelRepository;
        private readonly IIndicatorRepository _indicatorRepository;
        private readonly IControlPanelRepository _controlPanelRepository;
        public IConfiguration Configuration { get; }
        public DatapointExpressionController(IHostingEnvironment hostingEnvironment, IIndicatorRepository indicatorRepository, IConfiguration configuration, IPanelRepository panelRepository, IControlPanelRepository controlPanelRepository)
        {
            Configuration = configuration;
            _panelRepository = panelRepository;
            _indicatorRepository = indicatorRepository;
            _controlPanelRepository = controlPanelRepository;
        }
        public IActionResult AddExpression()
        {
            ViewBag.Sector = _panelRepository.ViewPanel().Result;
            return View();
        }
        public IActionResult ViewExpression()
        {
            ViewBag.Sector = _panelRepository.ViewPanel().Result;
            ViewBag.Result= _controlPanelRepository.GetAllExpressionData();
            return View();
        }
        [HttpPost]
        public IActionResult ViewExpression(ABP.Domain.DataPointExpression.DatapointExpression DEX)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.ExpressionID = DEX.ExpressionID;
                ViewBag.SECTORID = DEX.SECTORID;
                ViewBag.Sector = _panelRepository.ViewPanel().Result;
                ViewBag.Result = _controlPanelRepository.GetExpressionDataById(DEX.ExpressionID, DEX.SECTORID);//getting all indicator data to bind in the grid
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult ExpressionGateById(string ExpressionId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Expressions = _controlPanelRepository.GetExpressionDataById(Convert.ToInt32(ExpressionId),0).Result;
                return Ok(JsonConvert.SerializeObject(Expressions));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpPost]
        public IActionResult DeleteExpression(int id)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    DatapointExpression model = new DatapointExpression();
                    model.ExpressionID = id;
                    model.CREATEDBY = int.Parse(value.ToString());
                    string Result = _controlPanelRepository.DeleteExpression(model);
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
        [HttpPost]
        public async Task<JsonResult> AddExpression(DatapointExpression dataPoint)
        {
            DatapointExpression controlp = new DatapointExpression();
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
                   
                    try
                    {
                        //Getting values from form to Insert/Update into DB
                        List<ABP.Domain.Panel.ControlPanel> controlesList = new List<Domain.Panel.ControlPanel>();
                        string Expression = dataPoint.ExpressionNAME;
                        string ContolExpression = dataPoint.ExpressionNAME;
                        string ContolIDExpression = dataPoint.ExpressionNAME;
                        var Contollist = _controlPanelRepository.ControlPanelGateByPanelId(dataPoint.SECTORID,0).Result;
                        string NewContolExpression = "";
                        foreach (var l in Contollist)
                        {
                            if (ContolExpression.Contains(l.DISPLAYNAME))
                            {
                                ContolExpression = ContolExpression.Replace(l.DISPLAYNAME, "$('#"+l.DISPLAYNAME1+ "').val()");




                            }


                        }
                        foreach (var l in Contollist)
                        {
                            if (ContolIDExpression.Contains(l.DISPLAYNAME))
                            {
                                ContolIDExpression = ContolIDExpression.Replace(l.DISPLAYNAME, Convert.ToString(l.CONTROLID));




                            }


                        }

                        string NewExpression = "";
                        List<ABP.Domain.DataPointExpression.Operatorlist> OpList = new List<ABP.Domain.DataPointExpression.Operatorlist>();
                        OpList.Add(new ABP.Domain.DataPointExpression.Operatorlist { Discription  = " Should be less than or equals to ", Operator = "<=" });
                        OpList.Add(new ABP.Domain.DataPointExpression.Operatorlist { Discription = " Should be less than ", Operator = "<" });
                        OpList.Add(new ABP.Domain.DataPointExpression.Operatorlist { Discription = " Should be greater than or equals to ", Operator = ">=" });
                        OpList.Add(new ABP.Domain.DataPointExpression.Operatorlist { Discription = " Should be greater than ", Operator = ">" });
                        OpList.Add(new ABP.Domain.DataPointExpression.Operatorlist { Discription = " Should equals to ", Operator = "==" });
                        OpList.Add(new ABP.Domain.DataPointExpression.Operatorlist { Discription = " Should not equals to ", Operator = "!=" });
                       

                        foreach (var l in OpList)
                        {
                            if(Expression.Contains(l.Operator))
                            {
                                NewExpression= Expression.Replace(l.Operator, l.Discription);
                                ContolIDExpression = ContolIDExpression.Replace(l.Operator, Convert.ToString(","));


                                break;
                            }
                            
                            
                        }
                        string script = "if(" + ContolExpression + "){[Next]} else {bootbox.alert('" + NewExpression + "'); } ";
                        dataPoint.Script = script;
                        dataPoint.ExpressionNAMEWithID = ContolIDExpression;

                        dataPoint.CREATEDBY = Convert.ToInt32(value);
                        string retMsg = _controlPanelRepository.InsertControlExpression(dataPoint);
                        if (retMsg == "1")
                        {
                           
                                return Json("Record Saved Successfully");
                           
                        }
                        if (retMsg == "2")
                        {

                            return Json("Record Updated Successfully");

                        }
                        else
                        {
                            return Json("Record Already Exist");
                        }
                    }
                    catch (Exception ex)
                    {
                       // Logger.LogError(ex, ex.Message);
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
                public IActionResult BindControles(int Sectorid)
      {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var result = _controlPanelRepository.ControlPanelGateByPanelId(Sectorid,0).Result;
                    string jsonnresult = JsonConvert.SerializeObject(result);
                    //  ViewBag.Childlist = _ApprovalRepository.GetFETCH(DeptId, 0);
                    return Json(jsonnresult);
                    // return Json(searchlist);
                }
                else
                {
                    return RedirectToAction("Logout", "Home");
                }
            }
            catch { }
            return null;
        }
    }
}
