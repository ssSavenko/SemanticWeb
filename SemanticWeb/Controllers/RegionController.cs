using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SemanticWeb.Helpers;
using SemanticWeb.ViewModel;

namespace SemanticWeb.Controllers
{ 
    [ApiController]
    [Route("[controller]")]
    public class RegionController : Controller
    {
        ITokenHelper tokenHelper;
        IDBPediaHelper dBPediaHelper;

        public RegionController(IDBPediaHelper dBPediaHelper, ITokenHelper tokenHelper)
        {
            this.tokenHelper = tokenHelper;
            this.dBPediaHelper = dBPediaHelper;
        }

        [Route("GetRegions")]
        [HttpGet]
        public IActionResult GetRegions(string token)
            {
            if (!tokenHelper.IsTokenAvailable(token))
                return new ObjectResult("unknownUser");
            var result = dBPediaHelper.GetRegionsData();
            return new ObjectResult(result);
        }


        [Route("GetParksInRegion")]
        [HttpGet]
        public IActionResult GetParksInRegion(string token, string regionLink)
        {
            if (!tokenHelper.IsTokenAvailable(token))
                return new ObjectResult("unknownUser");
            var result = dBPediaHelper.GetRegionsParksData(regionLink);
            return new ObjectResult(result);
        }

        [Route("GetParksData")]
        [HttpGet]
        public IActionResult GetParksData(string token, string parkLink)
        {
            if (!tokenHelper.IsTokenAvailable(token))
                return new ObjectResult("unknownUser");
            var result = dBPediaHelper.GetParkData(parkLink);
            return new ObjectResult(result);
        }
    }
}
