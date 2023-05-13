using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Authorize(Roles = "HOMEADMIN")]
    [Area("admin")]
    [Route("api/[area]/[controller]")]

    [ApiController]
    public class HomeAdminController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<List<string>>> Index(List<string> body)
        {
            return  body;
        }
    }
}
