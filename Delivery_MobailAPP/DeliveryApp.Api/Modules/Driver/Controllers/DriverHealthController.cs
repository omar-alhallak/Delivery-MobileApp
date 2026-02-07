using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.API.Modules.Driver.Controllers
{
    [Route("api/driver/health")]
    [ApiController]
    public class DriverHealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "ok", module = "driver" });
    }
}
