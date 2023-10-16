using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ABP.Domain.Block;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.SendMessage;
using ABP.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ABP.Web.Controllers
{
    public class BlockController : Controller
    {
        private readonly IBlockRepository _BlockRepository;
        private readonly IDistrictRepository _DistrictRepository;
        public IConfiguration Configuration { get; }
        private readonly ILogger<MessageRepository> Logger;
        public BlockController(IConfiguration configuration, ILogger<MessageRepository> logger, IBlockRepository blockRepository, IDistrictRepository districtRepository)
        {
            _BlockRepository = blockRepository;
            _DistrictRepository = districtRepository;
            Configuration = configuration;
            Logger = logger;
            Logger.LogInformation("BlockController initialized");
        }
        [HttpGet]
        public IActionResult AddBlock(int Glink, int Plink)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                ViewBag.District = _DistrictRepository.GetDistrictAsync().Result;//Getting Districts For Binding Initials
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");  
            }
        }
        [HttpPost]
        public IActionResult MapBlock(string BlockData,int DistIdD)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                                
                string[] DistId = BlockData.Split(",");
                XElement identity = new XElement("person");
                foreach (string item in DistId)
                {
                    string[] DistidDLF = item.Split("|");

                    XElement elm = new XElement("row",
                     new XElement("BLOCKID", DistidDLF[0]),
                     new XElement("DELETEDFLAG", DistidDLF[1]));
                    identity.Add(elm);
                }
                var Result = _BlockRepository.InsertBlock(identity, Convert.ToInt32(DistIdD));
                return Json(Result);
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpGet]
        public IActionResult DeleteBlock(int DistCode)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                if (DistCode != 0)
                {
                    string Result = _BlockRepository.DeleteBlock(Convert.ToInt32(DistCode));//Unmapping block by updating respective dist code zero
                    if (Result == "3")
                    {
                        return Json("Record Deleted Successfully");
                    }
                    if (Result == "4")
                    {
                        return Json("Blocks Already In Use!");
                    }
                    else
                    {
                        return Json("Error Occureed!");
                    }
                }
                else
                {
                    return Json("Error Occureed!");
                }
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult ViewBlock()
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var ACTION = "VIEW";
                ViewBag.District = _DistrictRepository.GetDistrictAsync().Result;//To bind drop down initial
                ViewBag.Blocks = _BlockRepository.GetBlockByDist(ACTION,Convert.ToInt32("0"));
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetAllBlocks()
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Blocks = _BlockRepository.GetBlockAsync().Result;//Rendering block data in ViewBlock Page
                return Ok(JsonConvert.SerializeObject(Blocks));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult GetBlocksByDist(string DistCode)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");//Getting Block By DistCode
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var ACTION = "GET";
                var Blocks = _BlockRepository.GetBlockByDist(ACTION,Convert.ToInt32(DistCode)).Result;
                return Ok(JsonConvert.SerializeObject(Blocks));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpGet]
        public IActionResult GetMappedBlocksByDist(string DistCode)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");//Getting Block By DistCode
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Blocks1 = _BlockRepository.GetMappedeBlockByDist(Convert.ToInt32(DistCode)).Result;
                return Ok(JsonConvert.SerializeObject(Blocks1));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpGet]
        public IActionResult GetAllBlocksSearch(string DistCode)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Blocks = _BlockRepository.GetBlockDetailsByID(Convert.ToInt32(DistCode)).Result;//Costum Search By Dist Code
                return Ok(JsonConvert.SerializeObject(Blocks));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public IActionResult GetBlockByDistModal(string DistCode)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var Blocks = _BlockRepository.GetBlockByDistModal(Convert.ToInt32(DistCode)).Result;//Getting Blocks for modal view by clicking on count
                return Ok(JsonConvert.SerializeObject(Blocks));
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewBlock(Block block)
        {
            ViewBag.Url = Configuration.GetValue<string>("MySettings:Url");
            ViewBag.SSOUrl = Configuration.GetValue<string>("MySettings:SSOUrl");
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                var ACTION = "VIEW";
                ViewBag.District = _DistrictRepository.GetDistrictAsync().Result;//To bind drop down initial
                ViewBag.Blocks = _BlockRepository.GetBlockByDist(ACTION,Convert.ToInt32(block.DISTRICT_CODE));
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
    }
}