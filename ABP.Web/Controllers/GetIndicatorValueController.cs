using ABP.Domain.Indicator;
using ABP.Repository.Contract.Contract.Indicator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetIndicatorValueController : ControllerBase
    {
        private readonly IIndicatorRepository _indicatorRepository;
        public GetIndicatorValueController(IHostingEnvironment hostingEnvironment, IIndicatorRepository indicatorRepository)
        {
            _indicatorRepository = indicatorRepository;           
            Log.Information("GetIndicatorValueController initialized");
        }
        [HttpGet("IndicatorValue")]
        public IActionResult IndicatorValue()
        {
            try
            {                             
                var value = _indicatorRepository.IndicatorValues();               
                return Ok(JsonConvert.SerializeObject(value));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
